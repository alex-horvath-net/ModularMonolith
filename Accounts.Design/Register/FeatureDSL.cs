using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public abstract class FeatureDSL : ModuleDSL {
    internal async Task Run() {
        UserStory = new UserStory(AccountRepository, Hasher, Clock);
        Response = await UserStory.Register(Request, Token);
    }

    internal UserStory UserStory { get; set; } = null!;
    internal Request Request { get; set; } = null!;
    internal Response Response { get; set; } = null!;

    internal Func<Request> RequestFactory { get; set; } = null!;

    internal Task<TException> ShouldThrowAsync<TException>() where TException : Exception {
        var x = () => Task.CompletedTask;
        Run.ShouldThrowAsync<Exception>();
        return Assert.ThrowsAsync<TException>(Run);
    }

    internal Task ShouldNotThrowAsync() =>
        Run();

    // Given ***********************************************************************
    public void But() { }
    public void And() { }
    public FeatureDSL Given(params Action[] settings) {
        foreach (var setting in settings)
            setting();
        return this;
    }

    public void DefaultSettings() {

        TokenIsDefault();
        AccountRepositoryIsDefault();
        HasherFactoryIsDefault();
        ClockFactoryIsDefault();

        RequestIsDefault();
    }

    public void RequestIsDefault() => RequestFactory = () => new Request(
        Email: EmailFactory(),
        UserName: UserNameFactory(),
        Password: PasswordFactory(),
        Roles: RolesFactory());

    public void RequestIsMissing() => RequestFactory = () => null!;

    public void EmailIsMissing() => EmailFactory = () => null!;

    public void EmailIsNotNormalized() => EmailFactory = () => " Test-Trader@Bank.Com  ";

    public void PasswordIsMissing() => PasswordFactory = () => null!;
    public void PasswordIsShorterThan(int trashold) => PasswordFactory = () => PasswordFactory()[..(trashold - 1)];

    public void PasswordHasNoUpperCase() => PasswordFactory = () => PasswordFactory().ToLowerInvariant();

    public void PasswordHasNoLowerCase() => PasswordFactory = () => PasswordFactory().ToUpperInvariant();

    public void PasswordHasNoDigit() => PasswordFactory = () => new string(PasswordFactory().Where(c => !char.IsDigit(c)).ToArray());
    public void PasswordHasNoSpecialCharacter() => PasswordFactory = () => new string(PasswordFactory().Where(c => char.IsLetterOrDigit(c)).ToArray());

    public void UserNameIsMissing() => UserNameFactory = () => null!;

    public void UserNameIsNotNormalized() => UserNameFactory = () => " Test-Trader ";

    public void RolesIsMissing() => RolesFactory = () => null!;

    public void RolesAreNotNormailized() => RolesFactory = () => [null!, "", " "];
    public void RolesAreNotNormalized() => RolesFactory = () => [null!, "", " ", "Trader", " TradeR "];

    public void RolesContainUnregistered() => RolesFactory = () => ["Trader", "UnRegisteredRole"];

    public void AccountAlreadyExistsWithSimilarEmail() =>  {
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
        Request = RequestFactory!();
        Token = TokenFactory!();

        AccountRepository = AccountRepositoryFactory!();
        Hasher = HasherFactory!();
        Clock = ClockFactory!();

        var unitCommand = unitCommandFactory();
        await unitCommand;
        return this;
    }

    // Then ***********************************************************************
    //public void Then(Func<Task> action) => action();

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