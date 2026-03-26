using System.Globalization;
using Accounts.Core.Domain;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public abstract class FeatureDSL : ModuleDSL {
    protected FeatureDSL() {
        requestFactory = () => new Request(
            Email: emailFactory(),
            UserName: userNameFactory(),
            Password: passwordFactory(),
            Roles: rolesFactory());
    }

    internal async Task Run() {
        userStory = new UserStory(accountRepository, hasher, clock);
        response = await userStory.Register(request, token);
    }

    private void GenerateDependencies() {
        request = requestFactory!();
        token = tokenFactory!();

        accountRepository = accountRepositoryFactory!();
        hasher = hasherFactory!();
        clock = clockFactory!();
    }

    internal Func<Task> SUT { get; set; } = null!;
    internal UserStory userStory = null!;
    internal Request request = null!;
    internal Response response = null!;

    internal Func<Request> requestFactory = null!;

    // Given ***********************************************************************
    public void But() { }
    public void And() { }
    public FeatureDSL Given(params Action[] settings) {
        foreach (var setting in settings)
            setting();
        return this;
    }

    public void DefaultSettings() { }

    public void RequestIsMissing() => requestFactory = () => null!;

    public void EmailIsMissing() => emailFactory = () => null!;

    public void EmailIsNotNormalized() => emailFactory = () => " Test-Trader@Bank.Com  ";

    public void PasswordIsMissing() => passwordFactory = () => null!;
    public void PasswordIsShorterThan(int trashold) {
        var createPassword = passwordFactory;
        passwordFactory = () => createPassword()[..(trashold - 1)];
    }

    public void PasswordHasNoUpperCase() {
        var createPassword = passwordFactory;
        passwordFactory = () => createPassword().ToLowerInvariant();
    }

    public void PasswordHasNoLowerCase() {
        var createPassword = passwordFactory;
        passwordFactory = () => createPassword().ToUpperInvariant();
    }

    public void PasswordHasNoDigit() {
        var createPassword = passwordFactory;
        passwordFactory = () => new string(createPassword().Where(c => !char.IsDigit(c)).ToArray());
    }

    public void PasswordHasNoSpecialCharacter() {
        var createPassword = passwordFactory;
        passwordFactory = () => new string(createPassword().Where(char.IsLetterOrDigit).ToArray());
    }

    public void UserNameIsMissing() => userNameFactory = () => null!;

    public void UserNameIsNotNormalized() => userNameFactory = () => " Test-Trader ";

    public void RolesIsMissing() => rolesFactory = () => null!;

    public void RolesAreNotNormailized() => rolesFactory = () => [null!, "", " "];
    public void RolesAreNotNormalized() => rolesFactory = () => ["Trader", "trader"];

    public void RolesContainUnregistered() => rolesFactory = () => ["Trader", "UnRegisteredRole"];

    public void AccountAlreadyExistsWithSimilarEmail() {
        var existingAccount = new Account(
            Guid.NewGuid(),
            emailFactory(),
            userNameFactory(),
            passwordFactory(),
            rolesFactory().ToHashSet(StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

        var mock = accountRepositoryFactory!();
        mock.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existingAccount);
        accountRepositoryFactory = () => mock;
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