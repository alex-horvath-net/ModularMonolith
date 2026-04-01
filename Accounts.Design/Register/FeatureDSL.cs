using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public abstract class FeatureDSL : ModuleDSL<FeatureDSL> {
    private UserStory userStory = null!;
    private Request request = null!;
    private Response response = null!;

    protected async Task Run() {
        userStory = new UserStory(accountRepository, hasher, clock);
        response = await userStory.Register(request, token);
    }
    protected override void DefaultSettings() {
        base.DefaultSettings();

        RequestFactory = () => new Request(
            Email: EmailFactory(),
            UserName: UserNameFactory(),
            Password: PasswordFactory(),
            Roles: RolesFactory());
    }

    protected override void GenerateDependencies() {
        request = RequestFactory();
        token = TokenFactory();

        accountRepository = AccountRepositoryFactory();
        hasher = HasherFactory();
        clock = ClockFactory();
    }
    protected void RegistrationShouldBeAccepted() =>
        ShouldNotThrowException();

    protected void ClientShouldBeTold() =>
        ShouldThrowException();

    protected void ClientShouldBeTold(string message) =>
        ShouldThrow<InvalidOperationException>(message);

    protected void ClientShouldReceiveRegisteredIdentity() {
        response.AccountId.ShouldNotBe(Guid.Empty);
        response.Email.ShouldBe("test-trader@bank.com");
        response.UserName.ShouldBe("Test-Trader");
        response.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true);
    }

    protected void ClientShouldSeeEmail(string email) =>
        response.Email.ShouldBe(email);

    protected void ClientShouldSeeUserName(string userName) =>
        response.UserName.ShouldBe(userName);

    protected void ClientShouldSeeRoles(params string[] roles) =>
        response.Roles.ShouldBe(roles, ignoreOrder: true);

    protected void ExistingIdentityShouldBeChecked() =>
        accountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", token);

    protected void NewIdentityShouldBeStored() =>
        accountRepository.Received(1).CreateAccount(Arg.Any<Account>(), token);

    protected void NewIdentityShouldProtectCredentials() {
        hasher.Received(1).Generate(request.Password);
        accountRepository.Received(1).CreateAccount(Arg.Is<Account>(account => account.PasswordHash == "hashed-password"), token);
    }

    protected void NewIdentityShouldBeBuiltFromNormalizedClientData() =>
        accountRepository.Received(1).CreateAccount(Arg.Is<Account>(account =>
            account.Id != Guid.Empty &&
            account.Email == "test-trader@bank.com" &&
            account.UserName == "Test-Trader" &&
            account.PasswordHash == "hashed-password" &&
            account.Roles.Count == 1 &&
            account.Roles.Contains("Trader") &&
            account.CreatedAtUtc == DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture)), token);

    protected void ClientShouldRemainInControlWhileRegistrationIsStored() {
        token.ShouldNotBe(CancellationToken.None);
        token.IsCancellationRequested.ShouldBeTrue();
        accountRepository.Received(1).CreateAccount(Arg.Any<Account>(), token);
    }

    protected void StopBeforeDeduplication() =>
        accountRepository.DidNotReceiveWithAnyArgs().FindAccountByEmail(default!, default);

    protected void RegistrationShouldStopBeforeProtectingCredentials() =>
        hasher.DidNotReceiveWithAnyArgs().Generate(default!);

    protected void RegistrationShouldStopBeforeStoringNewIdentity() =>
        accountRepository.DidNotReceiveWithAnyArgs().CreateAccount(default!, default);

    protected void RegistrationShouldFollowThePromisedWorkflow() =>
        Received.InOrder(() => {
            accountRepository.FindAccountByEmail("test-trader@bank.com", token);
            hasher.Generate(request.Password);
            accountRepository.CreateAccount(Arg.Any<Account>(), token);
        });

    private protected Func<Request> RequestFactory { get; set; } = null!;

    protected void RequestIsMissing() => RequestFactory = () => null!;

    protected void RequestHasAnyIssue() => EmailIsMissing();
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

    protected void ClientProvidesWorkflowControl() =>
        TokenFactory = () => new CancellationToken(canceled: true);

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
        mock.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existingAccount);
        AccountRepositoryFactory = () => mock;
    }
}