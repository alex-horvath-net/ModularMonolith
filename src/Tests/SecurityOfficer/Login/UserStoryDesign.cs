using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Security;
using Experts.SecurityOfficer.Login;

namespace Tests.SecurityOfficer.Login;

public class UserStoryTests {
    [Fact]
    public async Task Login_Succeeds_ForRegisteredAccount() {
        var hasher = new Pbkdf2PasswordHasher();
        var account = new Account(
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            "aladar.horvath@outlook.com",
            "Aladar",
            hasher.Hash("P@ssw0rd!"),
            new HashSet<string>(["Trader"]),
            IsLocked: false,
            CreatedAtUtc: DateTime.UtcNow);

        var authenticate = new Authenticate(new FakeAuthenticateStore(account), hasher);
        var authorize = new Authorize();
        var userStory = new UserStory(authenticate, authorize);

        var request = new UserStory.Request(
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            UserStory.AccountType.LocalAccount,
            new Dictionary<string, string> {
                ["Email"] = "aladar.horvath@outlook.com",
                ["Password"] = "P@ssw0rd!"
            });

        var response = await userStory.Run(request, CancellationToken.None);

        Assert.Equal("Aladar", response.UserName);
        Assert.Null(response.ErrorMessage);
        Assert.Equal(account.Id, response.AuthenticationId);
    }

    private sealed class FakeAuthenticateStore(Account account) : Authenticate.IStore {
        public Task<Account?> FindByEmail(string email, CancellationToken token) => Task.FromResult(string.Equals(email, account.Email, StringComparison.OrdinalIgnoreCase) ? account : null);
    }

    private sealed class FakeAuthorizeStore(Account account) : Authorize.IStore {
        public Task<Account?> FindById(Guid id, CancellationToken token) => Task.FromResult(account.Id == id ? account : null);
    }
}
