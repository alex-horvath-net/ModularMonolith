using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Core.Infrastructure;

namespace Accounts.Design;

public class Fixtrure {
    protected IAccountRepository AccountantRepository { get; set; } = null!;
    protected IHasher Hasher { get; set; } = null!;
    protected IClock Clock { get; set; } = null!;
    protected CancellationToken Token { get; set; }

    protected Func<CancellationToken> TokenFactory { get; set; } = null!;
    protected Func<IAccountRepository> AccountRepositoryFactory { get; set; } = null!;
    protected Func<IHasher> HasherFactory { get; set; } = null!;
    protected Func<IClock> ClockFactory { get; set; } = null!;
    protected Func<string> UserNameFactory { get; set; } = null!;
    protected Func<string> PasswordFactory { get; set; } = null!;
    protected Func<string> EmailFactory { get; set; } = null!;
    protected Func<IReadOnlyCollection<string>> RolesFactory { get; set; } = null!;

    public Fixtrure() {

        TokenFactory = () => CancellationToken.None;

        AccountRepositoryFactory = () => {
            var mock = Substitute.For<IAccountRepository>();
            mock.FindAccountByEmail(default, default).Returns(Task.FromResult((Account?)null));
            mock.CreateAccount(default, default).Returns(Task.CompletedTask);
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

        EmailFactory = () => "Test-Trader@Bank.Com ";
        UserNameFactory = () => "  Test-Trader ";
        PasswordFactory = () => "Ab!123456789012";
        RolesFactory = () => ["Trader", "RiskManager"];
    }
}
