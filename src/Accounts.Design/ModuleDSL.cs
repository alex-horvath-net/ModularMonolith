using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Core.Infrastructure;
using Core.Infrastructure.GuidNumber;

namespace Accounts.Design;

public abstract class ModuleDSL<TFeatureDSL> where TFeatureDSL : ModuleDSL<TFeatureDSL> {
    protected IAccountRepository accountRepository = null!;
    protected IHasher hasher = null!;
    protected IGuidGenerator guidGenerator = null!;
    protected IClock clock = null!;
    protected CancellationToken token;
    protected CancellationTokenSource tokenSource = new();

    protected Exception? exception;

    protected virtual void ProdLike() {
        TokenFactory = () => tokenSource.Token;

        GuidFactory = () => new GuidGenerator();

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

    protected virtual void FastAndDeterministicDependencies() {

        GuidFactory = () => {
            var mock = Substitute.For<IGuidGenerator>();
            mock.New().Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));
            return mock;
        };

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

    protected virtual void GenerateDependencies() {
        hasher = HasherFactory();
        clock = ClockFactory();
        guidGenerator = GuidFactory();
        accountRepository = AccountRepositoryFactory();

        token = TokenFactory();
    }

    protected Func<IGuidGenerator> GuidFactory { get; set; } = null!;
    protected Func<CancellationToken> TokenFactory { get; set; } = null!;
    protected Func<IAccountRepository> AccountRepositoryFactory { get; set; } = null!;
    protected Func<IHasher> HasherFactory { get; set; } = null!;
    protected Func<IClock> ClockFactory { get; set; } = null!;
    protected Func<string> UserNameFactory { get; set; } = null!;
    protected Func<string> PasswordFactory { get; set; } = null!;
    protected Func<string> EmailFactory { get; set; } = null!;
    protected Func<IReadOnlyCollection<string>> RolesFactory { get; set; } = null!;

    protected void A() { }
    protected void With() { }
    protected void But() { }

    protected void And() { }

    protected TFeatureDSL Given(params Action[] dependecyFactories) {
        foreach (var dependecyFactory in dependecyFactories)
            dependecyFactory();

        return (TFeatureDSL)this;
    }

    internal async Task<TFeatureDSL> When(Func<Task> sut) {
        GenerateDependencies();

        await sut();

        return (TFeatureDSL)this;
    }

    internal void ShouldThrow<TException>(string? message = null) where TException : Exception {
        exception.ShouldNotBeNull();
        var typedException = exception.ShouldBeOfType<TException>();
        typedException.Message.ShouldBe(message);
    }

    internal void ShouldThrowException() => exception.ShouldNotBeNull();

    internal void ShouldNotThrowException() =>
         exception.ShouldBeNull();

}