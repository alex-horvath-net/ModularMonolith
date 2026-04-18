using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Register;

namespace Accounts.Design.Register;

public abstract class DSL : ModuleDSL<DSL> {
    private UserStory userStory = null!;
    private Request request = null!;
    private Response response = null!;

    protected IReadOnlyList<RegistrationWorkStep> workSteps = [];

    protected async Task Run() {
        userStory = new UserStory(accountRepository, hasher, clock, guidGenerator);
        var context = new Context(request, token);

        try {
            await userStory.Execute(context);
            response = context.ToResponse();
        } catch (Exception ex) {
            exception = ex;
        } finally {
            workSteps = context.ExecutedBusinessWorkSteps;
        }
    }
    protected override void ProdLike() {
        base.ProdLike();

        RequestFactory = () => new Request(
            Email: EmailFactory(),
            UserName: UserNameFactory(),
            Password: PasswordFactory(),
            Roles: RolesFactory(),
            CorrelationId: guidGenerator.New(),
            RequestId: guidGenerator.New());
    }

    protected override void GenerateDependencies() {
        base.GenerateDependencies();

        request = RequestFactory();
    }
    protected void RegisterUserStoryShouldBeAccepted() =>
        ShouldNotThrowException();

    protected void ProductOwnerShouldBeTold() =>
        ShouldThrowException();

    protected void ProductOwnerShouldBeTold(string message) =>
        ShouldThrow<InvalidOperationException>(message);

    protected void ProductOwnerShouldReceiveAUsableIdentity() {
        response.AccountId.ShouldNotBe(Guid.Empty);
        response.Email.ShouldBe("test-trader@bank.com");
        response.UserName.ShouldBe("Test-Trader");
        response.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true);
    }

    protected void ProductOwnerShouldSeeEmail(string email) =>
        response.Email.ShouldBe(email);

    protected void ProductOwnerShouldSeeUserName(string userName) =>
        response.UserName.ShouldBe(userName);

    protected void ProductOwnerShouldSeeRoles(params string[] roles) =>
        response.Roles.ShouldBe(roles, ignoreOrder: true);

    private protected Func<Request> RequestFactory { get; set; } = null!;

    protected void RequestIsMissing() => RequestFactory = () => null!;

    protected void RequestHasSomeIssue() => EmailIsMissing();
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

    protected void ProductOwnerProvidesBusinessWorkflowControl() =>
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