using System.Globalization;
using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Infrastructure.Security;
using Experts.SecurityOfficer.Register;
using NSubstitute;
using Shouldly;

namespace Tests.SecurityOfficer.Register;

public class RegisterUserStoryTests {
    private static UserStory CreateSut(UserStory.IAccountStore store, IRandomNumberGenerator random, UserStory.IClock clock) =>
        new(store, random, clock);

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials() {
        var store = Substitute.For<UserStory.IAccountStore>();
        store.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult<Account?>(null));
        Account? created = null;
        store.CreateAsync(Arg.Do<Account>(account => created = account), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        var random = Substitute.For<IRandomNumberGenerator>();
        var clock = Substitute.For<UserStory.IClock>();
        clock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

        var sut = CreateSut(store, random, clock);
        var request = new UserStory.Request(
            Email: "Trader@Bank.Com ",
            UserName: "  Jane Trader ",
            Password: "Sup3r$ecretPwd",
            Roles: ["Trader", "trader", "RiskManager"]);

        var response = await sut.Register(request, CancellationToken.None);

        response.Email.ShouldBe(request.Email);
        response.Roles.ShouldBe(request.Roles.Except(["trader"]));
        created.ShouldNotBeNull();
        created!.Email.ShouldBe(request.Email);
        created.UserName.ShouldBe(request.UserName);
        created.Roles.ShouldBe(request.Roles.Except(["trader"]));
        created.CreatedAtUtc.ShouldBe(clock.UtcNow);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_Throws() {
        var clock = Substitute.For<UserStory.IClock>();
        clock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));
        var existing = new Account(Guid.NewGuid(), "user@example.com", "Existing", "hash", new HashSet<string>(StringComparer.OrdinalIgnoreCase), false, clock.UtcNow);
        var store = Substitute.For<UserStory.IAccountStore>();
        store.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existing);
        var random = Substitute.For<IRandomNumberGenerator>();
        var sut = CreateSut(store, random, clock);
        var request = new UserStory.Request("user@example.com", "New", "Sup3r$ecretPwd", ["Trader"]);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.Register(request, CancellationToken.None));
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordIsWeak_Throws() {
        var store = Substitute.For<UserStory.IAccountStore>();
        var random = Substitute.For<IRandomNumberGenerator>();
        var clock = Substitute.For<UserStory.IClock>();
        clock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));
        var sut = CreateSut(store, random, clock);
        var request = new UserStory.Request("user@example.com", "New", "weak", ["Trader"]);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.Register(request, CancellationToken.None));
    }

}

