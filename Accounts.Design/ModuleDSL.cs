using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Core.Infrastructure;

namespace Accounts.Design;

public abstract class ModuleDSL {
    protected IAccountRepository AccountantRepository { get; set; } = null!;
    protected IHasher Hasher { get; set; } = null!;
    protected IClock Clock { get; set; } = null!;
    protected CancellationToken Token { get; set; }

    internal Func<CancellationToken> TokenFactory { get; set; } = () => CancellationToken.None;
    internal Func<IAccountRepository> AccountRepositoryFactory { get; set; } = () => {
        var mock = Substitute.For<IAccountRepository>();
        mock.FindAccountByEmail(default!, default).Returns(Task.FromResult((Account?)null));
        mock.CreateAccount(default!, default).Returns(Task.CompletedTask);
        return mock;
    };
    internal Func<IHasher> HasherFactory { get; set; } = () => {
        var mock = Substitute.For<IHasher>();
        mock.Generate(Arg.Any<string>()).Returns("hashed-password");
        return mock;
    };
    internal Func<IClock> ClockFactory { get; set; } = () => {
        var mock = Substitute.For<IClock>();
        mock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));
        return mock;
    };
    internal Func<string> UserNameFactory { get; set; } = () => "Test-Trader";
    internal Func<string> PasswordFactory { get; set; } = () => "Ab!456789012";
    internal Func<string> EmailFactory { get; set; } = () => "Test-Trader@Bank.com";
    internal Func<IReadOnlyCollection<string>> RolesFactory { get; set; } = () => ["Trader", "RiskManager"];

    protected abstract void Build();

    protected abstract Task<object> ExecuteUnit();

    internal T Set<T>(Action<T> change) where T : ModuleDSL {
        var clone = (T)MemberwiseClone();
        change(clone);
        return clone;
    }

    internal async Task<object> ExecuteAsync() {
        Build();
        return await ExecuteUnit();
    }

    internal Task<TException> ShouldThrowAsync<TException>() where TException : Exception =>
        Assert.ThrowsAsync<TException>(ExecuteAsync);

    internal Task ShouldNotThrowAsync() =>
        ExecuteAsync();
}