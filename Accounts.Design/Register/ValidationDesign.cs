using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class ValidationDesign : FeatureDSL {
    internal override UserStory Unit() => new(AccountantRepository, Hasher, Clock);
    internal override Task<Response> Call(UserStory userStory) => userStory.Register(Request, Token);
    internal override string WorkStep() => "Validation";

    [Fact]
    public Task Request_Should_Be_Presented() =>
        Given.RequestIsMissing().
        When.Register().
        Then.ShouldFailWith(Constants.RequestCanNotBeNell);

    [Fact]
    public Task Email_Should_Be_Presented() =>
        Given.EmailIsMissing().
        When.Register().
        Then.ShouldFailWith(Constants.EmailIsRequired);

    [Fact]
    public Task Password_Should_Be_Presented() =>
        Given.PasswordIsMissing().
        When.Register().
        Then.ShouldFailWith(Constants.PasswordMustBeContain);

    [Fact]
    public Task Password_Should_Be_Long() =>
        Given.PasswordIsShorterThan(12).
        When.Register().
        Then.ShouldFailWith(Constants.PasswordMustBeContain);

    [Fact]
    public Task Password_Should_Have_LowerCase() =>
        Given.PasswordHasNoLowerCase().
        When.Register().
        Then.ShouldFailWith(Constants.PasswordMustBeContain);

    [Fact]
    public Task Password_Should_Have_UpperCase() =>
         Given.PasswordHasNoUpperCase().
         When.Register().
         Then.ShouldFailWith(Constants.PasswordMustBeContain);

    [Fact]
    public Task Password_Should_Have_Digit() =>
         Given.PasswordHasNoDigit().
         When.Register().
         Then.ShouldFailWith(Constants.PasswordMustBeContain);

    [Fact]
    public Task Password_Should_Have_SpecialCharacter() =>
          Given.PasswordHasNoSpecialCharacter().
          When.Register().
          Then.ShouldFailWith(Constants.PasswordMustBeContain);

    [Fact]
    public Task UserName_Should_Be_Presented() =>
          Given.UserNameIsMissing().
          When.Register().
          Then.ShouldFailWith(Constants.UserNameIsRequired);

    [Fact]
    public Task Roles_Should_Be_Presented() =>
          Given.RolesIsMissing().
          When.Register().
          Then.ShouldFailWith(Constants.AtLeastOneRoleRequired);

    [Fact]
    public Task Roles_Should_Be_Registered() =>
         Given.RolesContainUnregistered().
         When.Register().
         Then.ShouldFailWith(Constants.AtLeastOneRoleRequired);
}