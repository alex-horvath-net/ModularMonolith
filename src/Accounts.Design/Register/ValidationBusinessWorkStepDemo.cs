using Accounts.Register;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class ValidationBusinessWorkStepDemo : DSL {
    [Fact]
    public Task ProductOwner_Can_Start_The_Register_User_Story_With_A_Valid_Request() =>
        Given(ProdLikeDependencies).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted);

    [Fact]
    public Task ProductOwner_Must_Provide_A_Request_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, RequestIsMissing).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.RequestCanNotBeNull));

    [Fact]
    public Task ProductOwner_Must_Provide_An_Email_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, EmailIsMissing).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.EmailIsRequired));

    [Fact]
    public Task ProductOwner_Must_Provide_A_Password_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, PasswordIsMissing).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task ProductOwner_Must_Provide_A_Long_Enough_Password_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, () => PasswordIsShorterThan(12)).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task ProductOwner_Must_Provide_A_Password_With_Lowercase_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, PasswordHasNoLowerCase).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task ProductOwner_Must_Provide_A_Password_With_Uppercase_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, PasswordHasNoUpperCase).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task ProductOwner_Must_Provide_A_Password_With_A_Digit_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, PasswordHasNoDigit).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task ProductOwner_Must_Provide_A_Password_With_A_Symbol_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, PasswordHasNoSpecialCharacter).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.PasswordMustBeContain));

    [Fact]
    public Task ProductOwner_Must_Provide_A_UserName_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, UserNameIsMissing).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.UserNameIsRequired));

    [Fact]
    public Task ProductOwner_Must_Provide_At_Least_One_Role_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, RolesIsMissing).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.AtLeastOneRoleRequired));

    [Fact]
    public Task ProductOwner_Must_Provide_Only_Registered_Roles_To_Start_The_Register_User_Story() =>
        Given(ProdLikeDependencies, But, RolesContainUnregistered).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.AtLeastOneRoleRequired));
}
