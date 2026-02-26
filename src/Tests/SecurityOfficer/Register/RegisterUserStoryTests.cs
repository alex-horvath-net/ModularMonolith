using System.Globalization;
using Core.Infrastructure;
using Features.Accounts.Domain;
using Features.Accounts.Infrastructure;
using Features.Accounts.Slices.Register.UserStory;
using NSubstitute;
using Shouldly;

namespace Tests.SecurityOfficer.Register;

public class RegisterUserStoryTests {
    private IAccountRepository repository = default!;
    private IHasher hasher = default!;
    private IClock clock = default!;
    private Request request = default!;
    private CancellationToken token;
    private Account? createdAccount;

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials() {
        var response = await Arrange().Register(request, token);

        response.Email.ShouldBe(request.Email);
        response.Roles.ShouldBe(request.Roles.Except(["trader"]));
        createdAccount.ShouldNotBeNull();
        createdAccount.Email.ShouldBe(request.Email);
        createdAccount.UserName.ShouldBe(request.UserName);
        createdAccount.Roles.ShouldBe(request.Roles.Except(["trader"]));
        createdAccount.CreatedAtUtc.ShouldBe(clock.UtcNow);
    }

    [Fact]
    public Task RegisterAsync_WhenEmailAlreadyExists_Throws() =>
        Should.ThrowAsync<InvalidOperationException>(() => Arrange(WithExistingAccount).Register(request, token));

    [Fact]
    public Task RegisterAsync_WhenPasswordIsWeak_Throws() =>
        Should.ThrowAsync<InvalidOperationException>(() => Arrange(() => new Request("user@example.com", "Generate", "weak", ["Trader"])).Register(request, token));

    private UserStory Arrange(Func<Request>? requestFactory = null) {
        token = CancellationToken.None;

        repository = Substitute.For<IAccountRepository>();
        repository.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Account?>(null));
        repository.CreateAccount(Arg.Do<Account>(account => createdAccount = account), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        hasher = Substitute.For<IHasher>();

        clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

        request = requestFactory == null ? DefaultRequest() : requestFactory();
        request = request with {
            Email = request.Email.Trim().ToLowerInvariant(),
            UserName = request.UserName.Trim(),
            Roles = request.Roles
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()
        };

        return new UserStory(repository, hasher, clock);
    }

    private Request DefaultRequest() => new(
        Email: "Trader@Bank.Com ",
        UserName: "  Jane Trader ",
        Password: "Sup3r$ecretPwd",
        Roles: ["Trader", "trader", "RiskManager"]);

    private Request WithExistingAccount() {
        var existingAccount = new Account(Guid.NewGuid(), "user@example.com", "Existing", "hash", new HashSet<string>(StringComparer.OrdinalIgnoreCase), false, clock.UtcNow);
        repository.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existingAccount);
        return new Request("user@example.com", "Generate", "Sup3r$ecretPwd", ["Trader"]);
    }

}

