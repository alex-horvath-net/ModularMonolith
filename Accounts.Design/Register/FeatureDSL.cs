using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public abstract class FeatureDSL : ModuleDSL {
    internal async Task Run() {
        // arrange 
        Request = RequestFactory();
        Token = TokenFactory();

        AccountRepository = AccountRepositoryFactory();
        Hasher = HasherFactory();
        Clock = ClockFactory();

        UserStory = new UserStory(AccountRepository, Hasher, Clock);

        // act   
        Response = await UserStory.Register(Request, Token);

        // assert

    }

    internal UserStory UserStory { get; set; } = null!;
    internal Request Request { get; set; } = null!;
    internal Response Response { get; set; } = null!;

    internal Func<Request> RequestFactory { get; set; }

    protected FeatureDSL() {

        RequestFactory = () => new Request(
            Email: EmailFactory(),
            UserName: UserNameFactory(),
            Password: PasswordFactory(),
            Roles: RolesFactory());
    }

    internal Task<TException> ShouldThrowAsync<TException>() where TException : Exception =>
       Assert.ThrowsAsync<TException>(Run);

    internal Task ShouldNotThrowAsync() =>
        Run();

    // Given ***********************************************************************
    public FeatureDSL Given(Func<FeatureDSL> factoryConfiguration) {
        factoryConfiguration();
        return this;
    }

    public FeatureDSL DefaultSettings() => this;
    public FeatureDSL RequestIsMissing() => Set<FeatureDSL>(x => x.RequestFactory = () => null!);

    public FeatureDSL EmailIsMissing() =>
        Set<FeatureDSL>(x => x.EmailFactory = () => null!);

    public FeatureDSL EmailIsNotNormalized() =>
        Set<FeatureDSL>(x => x.EmailFactory = () => " Test-Trader@Bank.Com  ");

    public FeatureDSL PasswordIsMissing() =>
        Set<FeatureDSL>(x => x.PasswordFactory = () => null!);

    public FeatureDSL PasswordIsShorterThan(int trashold) =>
        Set<FeatureDSL>(x => x.PasswordFactory = () => x.PasswordFactory()[..(trashold - 1)]);

    public FeatureDSL PasswordHasNoUpperCase() =>
        Set<FeatureDSL>(x => x.PasswordFactory = () => x.PasswordFactory().ToLowerInvariant());

    public FeatureDSL PasswordHasNoLowerCase() =>
        Set<FeatureDSL>(x => x.PasswordFactory = () => x.PasswordFactory().ToUpperInvariant());

    public FeatureDSL PasswordHasNoDigit() =>
        Set<FeatureDSL>(x => x.PasswordFactory = () => new string(x.PasswordFactory().Where(c => !char.IsDigit(c)).ToArray()));

    public FeatureDSL PasswordHasNoSpecialCharacter() =>
        Set<FeatureDSL>(x => x.PasswordFactory = () => new string(x.PasswordFactory().Where(c => char.IsLetterOrDigit(c)).ToArray()));

    public FeatureDSL UserNameIsMissing() =>
        Set<FeatureDSL>(x => x.UserNameFactory = () => null!);

    public FeatureDSL UserNameIsNotNormalized() =>
        Set<FeatureDSL>(x => x.UserNameFactory = () => " Test-Trader ");

    public FeatureDSL RolesIsMissing() =>
        Set<FeatureDSL>(x => x.RolesFactory = () => null!);

    public FeatureDSL RolesAreNotNormailized() =>
        Set<FeatureDSL>(x => x.RolesFactory = () => [null!, "", " "]);

    public FeatureDSL RolesAreNotNormalized() =>
        Set<FeatureDSL>(x => x.RolesFactory = () => [null!, "", " ", "Trader", " TradeR "]);

    public FeatureDSL RolesContainUnregistered() =>
        Set<FeatureDSL>(x => x.RolesFactory = () => ["Trader", "UnRegisteredRole"]);

    public FeatureDSL AccountAlreadyExistsWithSimilarEmail() =>
        Set<FeatureDSL>(x => {
            var existingAccount = new Account(
                Guid.NewGuid(),
                x.EmailFactory(),
                x.UserNameFactory(),
                x.PasswordFactory(),
                x.RolesFactory().ToHashSet(StringComparer.OrdinalIgnoreCase),
                IsLocked: false,
                CreatedAtUtc: DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

            var mock = x.AccountRepositoryFactory();
            mock.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existingAccount);
            x.AccountRepositoryFactory = () => mock;
        });

    // When ***********************************************************************
    public async Task<FeatureDSL> When(Func<Task> unitCommandFactory) {
        var unitCommand = unitCommandFactory();
        await unitCommand;
        return this;
    }

    // Then ***********************************************************************
    //public void Then(Func<Task> action) => action();

    public Task ShouldFailWithRequestCanNotBeNell() => ShouldFailWith(Constants.RequestCanNotBeNell);

    public async Task ShouldFailWith(string message) {
        var ex = await ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldBe(message);
    }

    public async Task ShouldFailWith(string message, Action<FeatureDSL> assertion) {
        var ex = await ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldBe(message);
        assertion(this);
    }

    public async Task ShouldSucceed() => await ShouldNotThrowAsync();

    public async Task ShouldSucceedWith(Action assertion) {
        await Run();
        assertion();
    }

    public async Task<FeatureDSL> UnitIsCalled() {
        await Run();
        return this;
    }
}