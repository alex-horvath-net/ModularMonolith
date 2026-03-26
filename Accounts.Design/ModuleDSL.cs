using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Core.Infrastructure;

namespace Accounts.Design;

public abstract class ModuleDSL {

    protected ModuleDSL() {
        TokenFactory = () => CancellationToken.None;

        AccountRepositoryFactory = () => {
            var mock = Substitute.For<IAccountRepository>();
            mock.FindAccountByEmail(default!, default).Returns(Task.FromResult((Account?)null));
            mock.CreateAccount(default!, default).Returns(Task.CompletedTask);
            return mock;
        };

        HasherFactory = () => {
            var mock = Substitute.For<IHasher>();
            mock.Generate(Arg.Any<string>()).Returns("hashed-password");
            return mock;
        };

        ClockFactory = () => {
            var mock = Substitute.For<IClock>();
            mock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));
            return mock;
        };

        UserNameFactory = () => "Test-Trader";

        PasswordFactory = () => "Ab!456789012";

        EmailFactory = () => "Test-Trader@Bank.com";

        RolesFactory = () => ["Trader", "RiskManager"];
    }

    internal IAccountRepository AccountRepository { get; set; } = null!;
    internal IHasher Hasher { get; set; } = null!;
    internal IClock Clock { get; set; } = null!;
    internal CancellationToken Token { get; set; }

    internal Func<CancellationToken>? TokenFactory { get; set; }
    internal Func<IAccountRepository>? AccountRepositoryFactory { get; set; }
    internal Func<IHasher>? HasherFactory { get; set; }
    internal Func<IClock>? ClockFactory { get; set; }
    internal Func<string> UserNameFactory { get; set; }
    internal Func<string> PasswordFactory { get; set; }
    internal Func<string> EmailFactory { get; set; }
    internal Func<IReadOnlyCollection<string>> RolesFactory { get; set; }
}