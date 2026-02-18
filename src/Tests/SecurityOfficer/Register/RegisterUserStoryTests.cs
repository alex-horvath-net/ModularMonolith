using System.Globalization;
using Business.Experts.SecurityOfficer.Domain;
using Business.Experts.SecurityOfficer.Infrastructure;
using Business.Experts.SecurityOfficer.UserStories.Register;
using NSubstitute;
using Shouldly;

namespace Tests.SecurityOfficer.Register;

public class RegisterUserStoryTests {
    private UserStory.IAccountStore store = default!;
    private IRandom random = default!;
    private UserStory.IClock clock = default!;
    private UserStoryRequest request = default!;
    private CancellationToken token = default!;
    private Account? createdAccount;

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials() {
        Arrange();

        var userStory = new UserStory(store, random, clock);
        var response = await userStory.Register(request, token);

        response.Email.ShouldBe(request.Email);
        response.Roles.ShouldBe(request.Roles.Except(["trader"]));
        createdAccount.ShouldNotBeNull();
        createdAccount!.Email.ShouldBe(request.Email);
        createdAccount.UserName.ShouldBe(request.UserName);
        createdAccount.Roles.ShouldBe(request.Roles.Except(["trader"]));
        createdAccount.CreatedAtUtc.ShouldBe(clock.UtcNow);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_Throws() {
        Arrange(WithExistingAccount);

        var userStory = new UserStory(store, random, clock);
        await Should.ThrowAsync<InvalidOperationException>(() => userStory.Register(request, token));
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordIsWeak_Throws() {
        Arrange(() => new UserStoryRequest("user@example.com", "Generate", "weak", ["Trader"]));

        var userStory = new UserStory(store, random, clock);
        await Should.ThrowAsync<InvalidOperationException>(() => userStory.Register(request, token));
    }

    private void Arrange(Func<UserStoryRequest>? requestFactory = null) {
        request = requestFactory == null ? DefaultRequest() : requestFactory();
        token = CancellationToken.None;

        store = Substitute.For<UserStory.IAccountStore>();
        store.FindByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Account?>(null));
        store.CreateAsync(Arg.Do<Account>(account => createdAccount = account), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        random = Substitute.For<IRandom>();

        clock = Substitute.For<UserStory.IClock>();
        clock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));
    }

    private UserStoryRequest DefaultRequest() => new(
        Email: "Trader@Bank.Com ",
        UserName: "  Jane Trader ",
        Password: "Sup3r$ecretPwd",
        Roles: ["Trader", "trader", "RiskManager"]);

    private UserStoryRequest WithExistingAccount() {
        var existing = new Account(Guid.NewGuid(), "user@example.com", "Existing", "hash", new HashSet<string>(StringComparer.OrdinalIgnoreCase), false, clock.UtcNow);
        store.FindByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existing);
        return new UserStoryRequest("user@example.com", "Generate", "Sup3r$ecretPwd", ["Trader"]);
    }

}

