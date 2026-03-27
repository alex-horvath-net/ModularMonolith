using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Core.Infrastructure;

namespace Accounts.Design;

public abstract class ModuleDSL<TFeatureDSL> where TFeatureDSL : ModuleDSL<TFeatureDSL> {
    protected virtual void DefaultSettings() {
        TokenFactory = () => CancellationToken.None;

        AccountRepositoryFactory = () => {
            var mock = Substitute.For<IAccountRepository>();
            mock.FindAccountByEmail(default!, default).Returns(Task.FromResult((Account?)null));
            mock.CreateAccount(default!, default).Returns(Task.CompletedTask);
            return mock;
        };

        HasherFactory = () => {
            var mock = Substitute.For<IHasher>();
            mock.Generate(default!).Returns("hashed-password");
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

    protected IAccountRepository AccountRepository { get; set; } = null!;
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

    protected Func<Task> SUT { get; set; } = null!;

    protected void But() { }

    protected void And() { }

    protected TFeatureDSL Given(params Action[] settings) {
        foreach (var setting in settings)
            setting();

        return (TFeatureDSL)this;
    }

    internal TFeatureDSL When(Func<Task> sut) {
        GenerateDependencies();
        SUT = sut;
        return (TFeatureDSL)this;
    }

    protected async Task ShouldThrow<TException>(string? message = null) where TException : Exception {
        var ex = await SUT.ShouldThrowAsync<TException>();
        if (message is not null)
            ex.Message.ShouldBe(message);
    }

    protected abstract void GenerateDependencies();
}