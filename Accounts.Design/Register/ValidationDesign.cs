using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class ValidationDesign : FeatureDSL {
    [Fact]
    public async Task Request_Should_Be_Presented() => await
        Given(RequestIsMissing).
        When(Run).
        Then(() => ShouldFailWith(Constants.RequestCanNotBeNell));

    [Fact]
    public async Task Email_Should_Be_Presented() => await
        Given(EmailIsMissing).
        When(Run).
        Then(() => ShouldFailWith(Constants.EmailIsRequired));

    [Fact]
    public async Task Password_Should_Be_Presented() => await
        Given(PasswordIsMissing).
        When(Run).
        Then(() => ShouldFailWith(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Be_Long() => await
        Given(() => PasswordIsShorterThan(12)).
        When(Run).
        Then(() => ShouldFailWith(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_LowerCase() => await
        Given(PasswordHasNoLowerCase).
        When(Run).
        Then(() => ShouldFailWith(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_UpperCase() => await
        Given(PasswordHasNoUpperCase).
        When(Run).
        Then(() => ShouldFailWith(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_Digit() => await
        Given(PasswordHasNoDigit).
        When(Run).
        Then(() => ShouldFailWith(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_SpecialCharacter() => await
        Given(PasswordHasNoSpecialCharacter).
        When(Run).
        Then(() => ShouldFailWith(Constants.PasswordMustBeContain));

    [Fact]
    public async Task UserName_Should_Be_Presented() => await
        Given(UserNameIsMissing).
        When(Run).
        Then(() => ShouldFailWith(Constants.UserNameIsRequired));

    [Fact]
    public async Task Roles_Should_Be_Presented() => await
        Given(RolesIsMissing).
        When(Run).
        Then(() => ShouldFailWith(Constants.AtLeastOneRoleRequired));

    [Fact]
    public async Task Roles_Should_Be_Registered() => await
        Given(RolesContainUnregistered).
        When(Run).
        Then(() => ShouldFailWith(Constants.AtLeastOneRoleRequired));
}