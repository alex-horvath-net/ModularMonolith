using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class ValidationDesign : ValidationDesignDSL {
    [Fact]
    public Task Client_Can_Start_Registration_With_A_Valid_Request() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegistrationShouldBeAccepted);

    [Fact]
    public Task Client_Must_Provide_A_Request() =>
        Given(DefaultSettings, But, RequestIsMissing).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.RequestCanNotBeNull));

    [Fact]
    public Task Client_Must_Provide_An_Email() =>
        Given(DefaultSettings, But, EmailIsMissing).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.EmailIsRequired));

    [Fact]
    public Task Client_Must_Provide_A_Password() =>
        Given(DefaultSettings, But, PasswordIsMissing).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task Client_Must_Provide_A_Long_Enough_Password() =>
        Given(DefaultSettings, But, () => PasswordIsShorterThan(12)).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task Client_Must_Provide_A_Password_With_Lowercase() =>
        Given(DefaultSettings, But, PasswordHasNoLowerCase).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task Client_Must_Provide_A_Password_With_Uppercase() =>
        Given(DefaultSettings, But, PasswordHasNoUpperCase).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task Client_Must_Provide_A_Password_With_A_Digit() =>
        Given(DefaultSettings, But, PasswordHasNoDigit).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task Client_Must_Provide_A_Password_With_A_Symbol() =>
        Given(DefaultSettings, But, PasswordHasNoSpecialCharacter).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task Client_Must_Provide_A_UserName() =>
        Given(DefaultSettings, But, UserNameIsMissing).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.UserNameIsRequired));

    [Fact]
    public Task Client_Must_Provide_At_Least_One_Role() =>
        Given(DefaultSettings, But, RolesIsMissing).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.AtLeastOneRoleRequired));

    [Fact]
    public Task Client_Must_Provide_Only_Registered_Roles() =>
        Given(DefaultSettings, But, RolesContainUnregistered).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.AtLeastOneRoleRequired));
}

public class ValidationDesignDSL : FeatureDSL {
}