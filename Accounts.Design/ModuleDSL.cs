using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Core.Infrastructure;

namespace Accounts.Design;

public abstract class ModuleDSL {

    protected ModuleDSL() {
        tokenFactory = () => CancellationToken.None;

        accountRepositoryFactory = () => {
            var mock = Substitute.For<IAccountRepository>();
            mock.FindAccountByEmail(default!, default).Returns(Task.FromResult((Account?)null));
            mock.CreateAccount(default!, default).Returns(Task.CompletedTask);
            return mock;
        };

        hasherFactory = () => {
            var mock = Substitute.For<IHasher>();
            mock.Generate(Arg.Any<string>()).Returns("hashed-password");
            return mock;
        };

        clockFactory = () => {
            var mock = Substitute.For<IClock>();
            mock.UtcNow.Returns(DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));
            return mock;
        };

        userNameFactory = () => "Test-Trader";

        passwordFactory = () => "Ab!456789012";

        emailFactory = () => "Test-Trader@Bank.com";

        rolesFactory = () => ["Trader", "RiskManager"];
    }

    internal IAccountRepository accountRepository = null!;
    internal IHasher hasher = null!;
    internal IClock clock = null!;
    internal CancellationToken token;

    internal Func<CancellationToken>? tokenFactory;
    internal Func<IAccountRepository>? accountRepositoryFactory;
    internal Func<IHasher>? hasherFactory;
    internal Func<IClock>? clockFactory;
    internal Func<string> userNameFactory;
    internal Func<string> passwordFactory;
    internal Func<string> emailFactory;
    internal Func<IReadOnlyCollection<string>> rolesFactory;
}