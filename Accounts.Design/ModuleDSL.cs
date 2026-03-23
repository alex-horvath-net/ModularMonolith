using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Core.Infrastructure;

namespace Accounts.Design;

public abstract class ModuleDSL {
    internal IAccountRepository AccountRepository { get; set; } = null!;
    internal IHasher Hasher { get; set; } = null!;
    internal IClock Clock { get; set; } = null!;
    internal CancellationToken Token { get; set; }
    internal Func<CancellationToken>? TokenFactory { get; set; }
    internal Func<IAccountRepository>? AccountRepositoryFactory { get; set; }
    internal Func<IHasher>? HasherFactory { get; set; }
    internal Func<IClock>? ClockFactory { get; set; }
    internal Func<string> UserNameFactory { get; set; } = () => "Test-Trader";
    internal Func<string> PasswordFactory { get; set; } = () => "Ab!456789012";
    internal Func<string> EmailFactory { get; set; } = () => "Test-Trader@Bank.com";
    internal Func<IReadOnlyCollection<string>> RolesFactory { get; set; } = () => ["Trader", "RiskManager"];

    public void TokenIsDefault() => TokenFactory = () => CancellationToken.None;
    public void AccountRepositoryIsDefault() => AccountRepositoryFactory = () => {
        var mock = Substitute.For<IAccountRepository>();
        mock.FindAccountByEmail(default!, default).Returns(Task.FromResult((Account?)null));
        mock.CreateAccount(default!, default).Returns(Task.CompletedTask);
        return mock;
    };

    public void HasherFactoryIsDefault() => HasherFactory = () => {
        var mock = Substitute.For<IHasher>();
        mock.Generate(Arg.Any<string>()).Returns("hashed-password");
        return mock;
    };

    public void ClockFactoryIsDefault() => ClockFactory = () => {
        var mock = Substitute.For<IClock>();
        mock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));
        return mock;
    };
}