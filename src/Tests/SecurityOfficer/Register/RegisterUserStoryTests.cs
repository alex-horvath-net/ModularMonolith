using Experts.SecurityOfficer.Register;
using Experts.SecurityOfficer.Shared.Domain;
using Experts.SecurityOfficer.Shared.Security;

namespace Tests.SecurityOfficer.Register;

public class RegisterUserStoryTests
{
    private readonly FakeStore store = new();
    private readonly FakeHasher hasher = new();
    private readonly FakeRolePolicy rolePolicy = new();
    private readonly FakeClock clock = new(DateTime.Parse("2024-01-01T00:00:00Z"));

    private UserStory CreateSut() => new(store, hasher, rolePolicy, clock);

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials()
    {
        var sut = CreateSut();
        var request = new UserStory.Request(
            Email: "Trader@Bank.Com ",
            UserName: "  Jane Trader ",
            Password: "Sup3r$ecretPwd",
            Roles: new[] { "Trader", "trader", "RiskManager" });

        var response = await sut.RegisterAsync(request);

        Assert.Equal("trader@bank.com", response.Email);
        AssertEquivalent(new[] { "Trader", "RiskManager" }, response.Roles);

        Assert.NotNull(store.Created);
        Assert.Equal("trader@bank.com", store.Created!.Email);
        Assert.Equal("Jane Trader", store.Created.UserName);
        AssertEquivalent(new[] { "Trader", "RiskManager" }, store.Created.Roles);
        Assert.Equal(clock.UtcNow, store.Created.CreatedAtUtc);
        Assert.Contains("Sup3r$ecretPwd", hasher.HashInputs);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_Throws()
    {
        store.Existing = new Account(Guid.NewGuid(), "user@example.com", "Existing", "hash", Array.Empty<string>(), false, clock.UtcNow);
        var sut = CreateSut();
        var request = new UserStory.Request("user@example.com", "New", "Sup3r$ecretPwd", new[] { "Trader" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordIsWeak_Throws()
    {
        var sut = CreateSut();
        var request = new UserStory.Request("user@example.com", "New", "weak", new[] { "Trader" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RegisterAsync(request));

        Assert.Null(store.Created);
    }

    private sealed class FakeStore : UserStory.IAccountStore
    {
        public Account? Existing { get; set; }
        public Account? Created { get; private set; }

        public Task<Account?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
            => Task.FromResult(normalizedEmail == Existing?.Email ? Existing : null);

        public Task CreateAsync(Account account, CancellationToken cancellationToken)
        {
            Created = account;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeHasher : IPasswordHasher
    {
        public List<string> HashInputs { get; } = [];

        public string Hash(string password)
        {
            HashInputs.Add(password);
            return "hashed-" + password;
        }

        public bool Verify(string password, string storedHash) => throw new NotSupportedException();
    }

    private sealed class FakeRolePolicy : UserStory.IRolePolicy
    {
        public bool AreEligible(IEnumerable<string> requestedRoles) => requestedRoles.All(r => r is "Trader" or "RiskManager" or "Compliance");
    }

    private sealed class FakeClock(DateTime now) : UserStory.IClock
    {
        public DateTime UtcNow => now;
    }

    private static void AssertEquivalent(IEnumerable<string> expected, IEnumerable<string> actual) => Assert.Equal(
            expected.OrderBy(x => x, StringComparer.OrdinalIgnoreCase),
            actual.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
}
