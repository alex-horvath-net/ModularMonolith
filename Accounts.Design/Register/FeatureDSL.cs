using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public abstract class FeatureDSL : ModuleDSL<FeatureDSL> {
    protected async Task Run() {
        UserStory = new UserStory(AccountRepository, Hasher, Clock);
        Response = await UserStory.Register(Request, Token);
    }

    private protected UserStory UserStory { get; set; } = null!;
    private protected Request Request { get; set; } = null!;
    private protected Response Response { get; set; } = null!;

    protected override void DefaultSettings() {
        base.DefaultSettings();

        RequestFactory = () => new Request(
            Email: EmailFactory(),
            UserName: UserNameFactory(),
            Password: PasswordFactory(),
            Roles: RolesFactory());
    }

    protected override void GenerateDependencies() {
        Request = RequestFactory();
        Token = TokenFactory();

        AccountRepository = AccountRepositoryFactory();
        Hasher = HasherFactory();
        Clock = ClockFactory();
    }

    private protected Func<Request> RequestFactory { get; set; } = null!;

    protected void RequestIsMissing() => RequestFactory = () => null!;

    protected void EmailIsMissing() => EmailFactory = () => null!;

    protected void EmailIsNotNormalized() => EmailFactory = () => " Test-Trader@Bank.Com  ";

    protected void PasswordIsMissing() => PasswordFactory = () => null!;
    protected void PasswordIsShorterThan(int trashold) {
        var createPassword = PasswordFactory;
        PasswordFactory = () => createPassword()[..(trashold - 1)];
    }

    protected void PasswordHasNoUpperCase() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => createPassword().ToLowerInvariant();
    }

    protected void PasswordHasNoLowerCase() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => createPassword().ToUpperInvariant();
    }

    protected void PasswordHasNoDigit() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => new string(createPassword().Where(c => !char.IsDigit(c)).ToArray());
    }

    protected void PasswordHasNoSpecialCharacter() {
        var createPassword = PasswordFactory;
        PasswordFactory = () => new string(createPassword().Where(char.IsLetterOrDigit).ToArray());
    }

    protected void UserNameIsMissing() => UserNameFactory = () => null!;

    protected void UserNameIsNotNormalized() => UserNameFactory = () => " Test-Trader ";

    protected void RolesIsMissing() => RolesFactory = () => null!;

    protected void RolesAreNotNormailized() => RolesFactory = () => [null!, "", " "];
    protected void RolesAreNotNormalized() => RolesFactory = () => ["Trader", "trader"];

    protected void RolesContainUnregistered() => RolesFactory = () => ["Trader", "UnRegisteredRole"];

    protected void AccountAlreadyExistsWithSimilarEmail() {
        var existingAccount = new Account(
            Guid.NewGuid(),
            EmailFactory(),
            UserNameFactory(),
            PasswordFactory(),
            RolesFactory().ToHashSet(StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

        var mock = AccountRepositoryFactory();
        mock.FindAccountByEmail(default!, default).Returns(existingAccount);
        AccountRepositoryFactory = () => mock;
    }
}