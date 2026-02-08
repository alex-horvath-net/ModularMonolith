using Common.Tasks;
using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Infrastructure.Security;
using Experts.SecurityOfficer.Login;
using Shouldly;

namespace Tests.SecurityOfficer.Login;

public class UserStoryTests {
    private UserStory userStory = default!;

    [Fact]
    public async Task Login_Should_Succeed_For_Registered_Account() {
        Arrange();

        var response = await userStory.Run(request, token);

        response.ErrorMessage.ShouldBeNull();
        response.AuthenticationId.ShouldBe(account.Id);
        response.UserName.ShouldBe(account.UserName);
        response.Roles.ShouldBe(account.Roles, ignoreOrder: true);
    }

    [Fact]
    public async Task Login_Should_Fail_If_Request_Wrong_Beacause_AccountType() {
        Arrange(WithRequestWithWrongAccountType);

        var response = await userStory.Run(request, token);

        response.ErrorMessage.ShouldBe(UserStoryConstants.AccountTypeNotFound);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Should_Fail_If_Request_Wrong_Beacause_Password_Missing() {
        Arrange(WithRequestWithoutPassword);

        var response = await userStory.Run(request, token);

        response.ErrorMessage.ShouldBe(UserStoryConstants.MissingPassword);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_Should_Fail_If_Request_Wrong_Beacause_Email_Missing() {
        Arrange(WithRequestWithotEmail);

        var response = await userStory.Run(request, token);

        response.ErrorMessage.ShouldBe(UserStoryConstants.MissingEmail);
        response.AuthenticationId.ShouldBeNull();
        response.UserName.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
    }


    private void Arrange(Func<UserStoryRequest>? requestFactory = null) {
        request = requestFactory == null ? DefaultRequest() : requestFactory();
        token = CancellationToken.None;

        account = DefaultAccount();
        authenticateStore = new FakeAuthenticateStore([account]);
        random = new FakeRandomNumberGenerator();
        userStory = new UserStory(authenticateStore, random);
    }

    private UserStoryRequest request = default!;
    private UserStoryRequest DefaultRequest() => new(
        VisitorId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
        AccountType: AccountType.LocalAccount,
        Credentials: new Dictionary<string, string> {
            ["Email"] = "alex.horvath.net@outlook.com",
            ["Password"] = "P@ssw0rd!"
        });
    private UserStoryRequest WithRequestWithWrongAccountType() => new(
      VisitorId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
      AccountType: AccountType.AzureAccount,
      Credentials: new Dictionary<string, string> {
          ["Email"] = "alex.horvath.net@outlook.com",
          ["Password"] = "P@ssw0rd!"
      });
    private UserStoryRequest WithRequestWithoutPassword() => new(
        VisitorId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
        AccountType: AccountType.LocalAccount,
        Credentials: new Dictionary<string, string> {
            ["Email"] = "alex.horvath.net@outlook.com"
        });
    private UserStoryRequest WithRequestWithotEmail() => new(
        VisitorId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
        AccountType: AccountType.LocalAccount,
        Credentials: new Dictionary<string, string> {
            ["Password"] = "P@ssw0rd!"
        });

    private CancellationToken token = default!;


    private Account account = default!;
    private Account DefaultAccount() => new(
        Id: Guid.Parse("20000000-0000-0000-0000-000000000001"),
        Email: "alex.horvath.net@outlook.com",
        UserName: "Alex",
        PasswordHash: "FG9LGgLaReKuqwOfARhgaO7cD2CGvOMuq641z3LqcX54sfiWZyFAdnWpLDgeL6/r", // hash of P@ssw0rd!
        Roles: new HashSet<string>(["Trader"], StringComparer.OrdinalIgnoreCase),
        IsLocked: false,
        CreatedAtUtc: DateTime.UtcNow);


    private IAuthenticateStore authenticateStore = default!;

    private IRandomNumberGenerator random = default!;

    private sealed class FakeAuthenticateStore(Account[] accounts) : IAuthenticateStore {
        public Task<Account?> FindByEmail(string email, CancellationToken token) {
            _ = token;
            return accounts.FirstOrDefault(account => account.Email == email).ToTask();
        }
    }

    private sealed class FakeRandomNumberGenerator : IRandomNumberGenerator {
        public void Fill(Span<byte> data) {
            foreach (ref var b in data) {
                b = 0x42; // deterministic byte for testing
            }
        }
    }
}
