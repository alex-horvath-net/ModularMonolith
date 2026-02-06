using Common.Tasks;
using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Security;
using Experts.SecurityOfficer.Login;
using Shouldly;

namespace Tests.SecurityOfficer.Login;

public class UserStoryTests {
    private Func<UserStory.Request> requestFactory = default!;
    private Func<Account> accountFactoy = default!;
    private Func<FakeAuthenticateStore> authenticateStoreFactory = default!;
    private Func<Pbkdf2PasswordHasher> hasherFactory = default!;
    private Func<Authenticate> authenticateFactory = default!;
    private Func<Authorize> authorizeFactory = default!;
    private Func<UserStory> userStoryFactory = default!;
    private Func<Task<UserStory.Response>> Act = default!;
    private Account account = default!;

    public UserStoryTests() {
        requestFactory = () => new UserStory.Request(
            VisitorId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
            AccountType: UserStory.AccountType.LocalAccount,
            Credentials: new Dictionary<string, string> {
                ["Email"] = "alex.horvath.net@outlook.com",
                ["Password"] = "P@ssw0rd!"
            });

        accountFactoy = () => new(
            Id: Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Email: "alex.horvath.net@outlook.com",
            UserName: "Alex",
            PasswordHash: "FG9LGgLaReKuqwOfARhgaO7cD2CGvOMuq641z3LqcX54sfiWZyFAdnWpLDgeL6/r", // hash of P@ssw0rd!
            Roles: new HashSet<string>(["Trader"], StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: DateTime.UtcNow);

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
        response.Roles.ShouldBe(account.Roles, ignoreOrder: true);
    }

    [Fact]
    public async Task Login_Fails_ForUnKnownAccountType() {
        requestFactory = () => requestFactory() with { AccountType = UserStory.AccountType.AzureAccount };

        var response = await Act();

        response.ErrorMessage.ShouldBe(Authenticate.Constants.AccountTypeNotFound);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Fails_ForMissingPasswordCredential() {
        requestFactory = () => {
            var request = requestFactory();
            var credentials = request.Credentials.ToDictionary();
            credentials.Remove(Authenticate.Constants.Password);
            return request with { Credentials = credentials };
        };

        var response = await Act();

        response.ErrorMessage.ShouldBe(Authenticate.Constants.MissingPassword);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Fails_ForMisingEmailCredential() {
        requestFactory = () => {
            var request = requestFactory();
            var credentials = request.Credentials.ToDictionary();
            credentials.Remove(Authenticate.Constants.Email);
            return request with { Credentials = credentials };
        };

        var response = await Act();

        response.ErrorMessage.ShouldBe(Authenticate.Constants.MissingEmail);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }


    private sealed class FakeAuthenticateStore(Account[] accounts) : Authenticate.IStore {
        public Task<Account?> FindByEmail(string email, CancellationToken token) {
            _ = token;
            return accounts.FirstOrDefault(account => account.Email == email).ToTask();
        }
    }
}
