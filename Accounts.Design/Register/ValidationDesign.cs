using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class ValidationDesign : FeatureDSL {
    [Fact]
    public Task Request_Which_Is_Valid_Should_Be_Accepted() =>
        Given(DefaultSettings).
        When(Run).
        Then(ShouldNotThrowException);

    [Fact]
    public async Task Request_Should_Be_Presented() => await
        Given(DefaultSettings, But, RequestIsMissing).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.RequestCanNotBeNull));

    [Fact]
    public async Task Email_Should_Be_Presented() => await
        Given(DefaultSettings, But, EmailIsMissing).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.EmailIsRequired));

    [Fact]
    public async Task Password_Should_Be_Presented() => await
        Given(DefaultSettings, But, PasswordIsMissing).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Be_Long() => await
        Given(DefaultSettings, But, () => PasswordIsShorterThan(12)).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_LowerCase() => await
        Given(DefaultSettings, But, PasswordHasNoLowerCase).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_UpperCase() => await
        Given(DefaultSettings, But, PasswordHasNoUpperCase).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_Digit() => await
        Given(DefaultSettings, But, PasswordHasNoDigit).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public Task Password_Should_Have_SpecialCharacter() =>
        Given(DefaultSettings, But, PasswordHasNoSpecialCharacter).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public Task UserName_Should_Be_Presented() =>
        Given(DefaultSettings, But, UserNameIsMissing).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.UserNameIsRequired));

    [Fact]
    public Task Roles_Should_Be_Presented() =>
        Given(DefaultSettings, But, RolesIsMissing).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.AtLeastOneRoleRequired));

    [Fact]
    public Task Roles_Should_Be_Registered() =>
        Given(DefaultSettings, But, RolesContainUnregistered).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.AtLeastOneRoleRequired));
}