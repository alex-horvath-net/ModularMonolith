using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public abstract class FeatureDSL : ModuleDSL {
    protected FeatureDSL() {
        DefaultSettings();
    }

    internal async Task Run() {
        UserStory = new UserStory(AccountRepository, Hasher, Clock);
        Response = await UserStory.Register(Request, Token);
    }

    private void GenerateDependencies() {
        Request = RequestFactory!();
        Token = TokenFactory!();

        AccountRepository = AccountRepositoryFactory!();
        Hasher = HasherFactory!();
        Clock = ClockFactory!();
    }

    internal Func<Task> SUT { get; set; } = null!;
    internal UserStory UserStory { get; set; } = null!;
    internal Request Request { get; set; } = null!;
    internal Response Response { get; set; } = null!;

    internal Func<Request> RequestFactory { get; set; } = null!;

    // Given ***********************************************************************
    public void But() { }
    public void And() { }
    public FeatureDSL Given(params Action[] settings) {
        foreach (var setting in settings)
            setting();
        return this;
    }

    public void DefaultSettings() => RequestIsDefault();

    public void RequestIsDefault() => RequestFactory = () => new Request(
        Email: EmailFactory(),
        UserName: UserNameFactory(),
        Password: PasswordFactory(),
        Roles: RolesFactory());

    public void RequestIsMissing() => RequestFactory = () => null!;

    public void EmailIsMissing() => EmailFactory = () => null!;

    public void EmailIsNotNormalized() => EmailFactory = () => " Test-Trader@Bank.Com  ";

    public void PasswordIsMissing() => PasswordFactory = () => null!;
    public void PasswordIsShorterThan(int trashold) {
        var createPassword = PasswordFactory;
        PasswordFactory = () => createPassword()[..(trashold - 1)];
    }

    public void PasswordHasNoUpperCase() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => createPassword().ToLowerInvariant();
    }

    public void PasswordHasNoLowerCase() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => createPassword().ToUpperInvariant();
    }

    public void PasswordHasNoDigit() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => new string(createPassword().Where(c => !char.IsDigit(c)).ToArray());
    }

    public void PasswordHasNoSpecialCharacter() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => new string(createPassword().Where(char.IsLetterOrDigit).ToArray());
    }

    public void UserNameIsMissing() => UserNameFactory = () => null!;

    public void UserNameIsNotNormalized() => UserNameFactory = () => " Test-Trader ";

    public void RolesIsMissing() => RolesFactory = () => null!;

    public void RolesAreNotNormailized() => RolesFactory = () => [null!, "", " "];
    public void RolesAreNotNormalized() => RolesFactory = () => ["Trader", "trader"];

    public void RolesContainUnregistered() => RolesFactory = () => ["Trader", "UnRegisteredRole"];

    public void AccountAlreadyExistsWithSimilarEmail() {
        var existingAccount = new Account(
            Guid.NewGuid(),
            EmailFactory(),
            UserNameFactory(),
            PasswordFactory(),
            RolesFactory().ToHashSet(StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

        var mock = AccountRepositoryFactory!();
        mock.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existingAccount);
        AccountRepositoryFactory = () => mock;
    }

    // When ***********************************************************************
    public FeatureDSL When(Func<Task> sut) {
        GenerateDependencies();
        SUT = sut;
        return this;
    }

    // Then ***********************************************************************
    //public void Then(Func<Task> action) => action();

    public async Task ShouldThrow<TException>(string? message = null) where TException : Exception {
        var ex = await SUT.ShouldThrowAsync<TException>();
        if (message is not null)
            ex.Message.ShouldBe(message);
    }
}