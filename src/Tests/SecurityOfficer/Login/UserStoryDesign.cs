using Common.Tasks;
using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Security;
using Experts.SecurityOfficer.Login;
using Shouldly;

namespace Tests.SecurityOfficer.Login;

public class UserStoryTests {
    [Fact]
    public async Task Login_Succeeds_ForRegisteredAccount() {
        var account = AccountOfAlex();
        var userStory = GetLoginUserStory(account);
        var request = GoodRequest();

        var response = await userStory.Run(request, CancellationToken.None);

        response.ErrorMessage.ShouldBeNull();
        response.AuthenticationId.ShouldBe(account.Id);
        response.UserName.ShouldBe(account.UserName);
        response.Roles.ShouldBe(account.Roles);
    }

    [Fact]
    public async Task Login_Fails_ForUnKnownAccountType() {
        var account = AccountOfAlex();
        var userStory = GetLoginUserStory(account);
        var request = GoodRequest() with { AccountType = UserStory.AccountType.AzureAccount };

        var response = await userStory.Run(request, CancellationToken.None);

        response.ErrorMessage.ShouldBe(AuthenticateConstants.AccountTypeNotFound);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Fails_ForMissingPasswordCredential() {
        var account = AccountOfAlex();
        var userStory = GetLoginUserStory(account);

        var request = GoodRequest();
        var cedentials = request.Credentials.ToDictionary();
        cedentials.Remove(LocalAccountConstants.Password);
        request = request with { Credentials = cedentials };

        var response = await userStory.Run(request, CancellationToken.None);

        response.ErrorMessage.ShouldBe(AuthenticateConstants.MissingPassword);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Fails_ForMisingEmailCredential() {
        var account = AccountOfAlex();
        var userStory = GetLoginUserStory(account);

        var request = GoodRequest();
        var cedentials = request.Credentials.ToDictionary();
        cedentials.Remove(LocalAccountConstants.Email);
        request = request with { Credentials = cedentials };

        var response = await userStory.Run(request, CancellationToken.None);

        response.ErrorMessage.ShouldBe(AuthenticateConstants.MissingEmail);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }



    private static UserStory GetLoginUserStory(Account account) {
        var authenticate = GetAuthenticate(account);
        var authorize = GetAuthorize();
        var login = new UserStory(authenticate, authorize);
        return login;
    }

    private static Authorize GetAuthorize() => new();

    private static Authenticate GetAuthenticate(Account account) {
        var authenticateStore = new FakeAuthenticateStore([account]);
        var hasher = new Pbkdf2PasswordHasher();
        var authenticate = new Authenticate(authenticateStore, hasher);
        return authenticate;
    }

    private static UserStory.Request GoodRequest() => new(
        Guid.Parse("10000000-0000-0000-0000-000000000001"),
            UserStory.AccountType.LocalAccount,
            new Dictionary<string, string> {
                ["Email"] = "alex.horvath.net@outlook.com",
                ["Password"] = "P@ssw0rd!"
            });
    private Account AccountOfAlex() => new(
        Id: Guid.Parse("20000000-0000-0000-0000-000000000001"),
        Email: "alex.horvath.net@outlook.com",
        UserName: "Alex",
        PasswordHash: "FG9LGgLaReKuqwOfARhgaO7cD2CGvOMuq641z3LqcX54sfiWZyFAdnWpLDgeL6/r", // hash of P@ssw0rd!
        Roles: new HashSet<string>(["Trader"]),
        IsLocked: false,
        CreatedAtUtc: DateTime.UtcNow);

    private sealed class FakeAuthenticateStore(Account[] accounts) : Authenticate.IStore {
        public Task<Account?> FindByEmail(string email, CancellationToken token) =>
            accounts.FirstOrDefault(account => account.Email == email).ToTask();
    }
}
