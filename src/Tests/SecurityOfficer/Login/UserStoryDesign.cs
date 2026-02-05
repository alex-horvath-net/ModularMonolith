using Common.Tasks;
using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Infrastructure.Data.Models;
using Experts.SecurityOfficer.Common.Security;
using Experts.SecurityOfficer.Login;
using Shouldly;

namespace Tests.SecurityOfficer.Login;

public class UserStoryTests {
    private Func<UserStory.Request> requestFactory;
    private Func<Experts.SecurityOfficer.Common.Infrastructure.Data.Models.Account> accountFactoy;
    private Func<FakeAuthenticateStore> authenticateStoreFactory;
    private Func<Pbkdf2PasswordHasher> hasherFactory;
    private Func<Authenticate> authenticateFactory;
    private Func<Authorize> authorizeFactory;
    private Func<UserStory> userStoryFactory;
    private Func<Task<UserStory.Response>> Act;
    private Experts.SecurityOfficer.Common.Infrastructure.Data.Models.Account account;

    public UserStoryTests() {
        requestFactory = () => new UserStory.Request(
            VisitorId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
            AccountType: UserStory.AccountType.LocalAccount,
            Credentials: new Dictionary<string, string> {
                ["Email"] = "alex.horvath.net@outlook.com",
                ["Password"] = "P@ssw0rd!"
            });

        accountFactoy = () => new Experts.SecurityOfficer.Common.Infrastructure.Data.Models.Account {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Email = "alex.horvath.net@outlook.com",
            UserName = "Alex",
            PasswordHash = "FG9LGgLaReKuqwOfARhgaO7cD2CGvOMuq641z3LqcX54sfiWZyFAdnWpLDgeL6/r", // hash of P@ssw0rd!
            Roles = new HashSet<string>(["Trader"]),
            IsLocked = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        authenticateStoreFactory = () => {
            account = accountFactoy();
            return new FakeAuthenticateStore([account]);
        };

        hasherFactory = () => new Pbkdf2PasswordHasher();

        authenticateFactory = () => {
            var authenticateStore = authenticateStoreFactory();
            var hasher = hasherFactory();
            return new Authenticate(authenticateStore, hasher);
        };

        authorizeFactory = () => new Authorize();

        userStoryFactory = () => {
            var authenticate = authenticateFactory();
            var authorize = authorizeFactory();
            return new UserStory(authenticate, authorize);
        };

        Act = async () => {
            var userStory = userStoryFactory();
            var request = requestFactory();
            var token = CancellationToken.None;
            return await userStory.Run(request, token);
        };
    }


    [Fact]
    public async Task Login_Succeeds_ForRegisteredAccount() {

        var response = await Act();

        response.ErrorMessage.ShouldBeNull();
        response.AuthenticationId.ShouldBe(account.Id);
        response.UserName.ShouldBe(account.UserName);
        response.Roles.ShouldBe(account.Roles);
    }

    [Fact]
    public async Task Login_Fails_ForUnKnownAccountType() {
        var userStory = UserStoryFactory.Create(account);
        var request = GoodRequest() with { AccountType = UserStory.AccountType.AzureAccount };

        var response = await userStory.Run(request, CancellationToken.None);

        response.ErrorMessage.ShouldBe(Authenticate.Constants.AccountTypeNotFound);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Fails_ForMissingPasswordCredential() {
        var userStory = UserStoryFactory.Create(account);

        var request = GoodRequest();
        var cedentials = request.Credentials.ToDictionary();
        cedentials.Remove(Authenticate.Constants.Password);
        request = request with { Credentials = cedentials };

        var response = await userStory.Run(request, CancellationToken.None);

        response.ErrorMessage.ShouldBe(Authenticate.Constants.MissingPassword);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Fails_ForMisingEmailCredential() {
        var userStory = UserStoryFactory.Create(account);

        var request = GoodRequest();
        var cedentials = request.Credentials.ToDictionary();
        cedentials.Remove(Authenticate.Constants.Email);
        request = request with { Credentials = cedentials };

        var response = await userStory.Run(request, CancellationToken.None);

        response.ErrorMessage.ShouldBe(Authenticate.Constants.MissingEmail);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }


    private sealed class FakeAuthenticateStore(Account[] accounts) : Authenticate.IStore {
        public Task<Account?> FindByEmail(string email, CancellationToken token) =>
            accounts.FirstOrDefault(account => account.Email == email).ToTask();
    }
}
