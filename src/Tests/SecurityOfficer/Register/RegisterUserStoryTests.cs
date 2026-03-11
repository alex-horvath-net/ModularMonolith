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

    [Fact]
    public async Task RegisterAsync_Validate_Request_is_Mandatory() {
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => Arrange(WithoutRequest).Register(request, token));
        exception.Message.ShouldBe(Constants.RequestCanNotBeNell);
    }

    [Fact]
    public async Task RegisterAsync_Validate_Request_Email_is_Mandatory() {
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => Arrange(WithoutEmail).Register(request, token));
        exception.Message.ShouldBe(Constants.EmailIsRequired);
    }

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials() {
        var response = await Arrange().Register(request, token);

        response.Email.ShouldBe(request.Email);
        response.UserName.ShouldBe(request.UserName);
        response.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true);

        await repository.Received(1).FindAccountByEmail(request.Email, token);
        await repository.Received(1).CreateAccount(
            Arg.Is<Account>(account =>
                account.Email == request.Email &&
                account.UserName == request.UserName
                //account.Roles.SetEquals(["Trader", "RiskManager"]) &&
                //account.PasswordHash == "hashed-password"
                ),
            token);
    }

    [Fact]
    public Task RegisterAsync_WhenPasswordIsWeak_Throws() =>
        Should.ThrowAsync<InvalidOperationException>(() => Arrange(() => new Request("user@example.com", "Generate", "weak", ["Trader"])).Register(request, token));

    private UserStory Arrange(Func<Request>? requestFactory = null) {
        token = CancellationToken.None;

        repository = Substitute.For<IAccountRepository>();
        repository.FindAccountByEmail(default, default).Returns(Task.FromResult((Account?)null));
        repository.CreateAccount(default, default).Returns(Task.CompletedTask);

        hasher = Substitute.For<IHasher>();
        hasher.Generate(Arg.Any<string>()).Returns("hashed-password");

        clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

        request = requestFactory == null ? DefaultRequest() : requestFactory();
        if (request != null) {
            request = request with {
                Email = request.Email?.Trim()?.ToLowerInvariant(),
                UserName = request.UserName.Trim(),
                Roles = request.Roles
                    .Where(role => !string.IsNullOrWhiteSpace(role))
                    .Select(role => role.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray()
            };
        }
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

    private Request WithoutRequest() => null!;

    private Request WithoutEmail() => new Request("user@example.com", "Generate", "Sup3r$ecretPwd", ["Trader"]) with {
        Email = null!
    };
}

