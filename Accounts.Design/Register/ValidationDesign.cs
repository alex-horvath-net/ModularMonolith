using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class ValidationDesign : FeatureDSL {
    [Fact]
    public Task Request_Which_Is_Valid_Should_Be_Accepted() =>
        Given(DefaultSettings).
        When(Run).
        Next(ShouldNotThrowException);

    [Fact]
    public async Task Request_Should_Be_Presented() => await
        Given(DefaultSettings, But, RequestIsMissing).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.RequestCanNotBeNull));

    [Fact]
    public async Task Email_Should_Be_Presented() => await
        Given(DefaultSettings, But, EmailIsMissing).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.EmailIsRequired));

    [Fact]
    public async Task Password_Should_Be_Presented() => await
        Given(DefaultSettings, But, PasswordIsMissing).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Be_Long() => await
        Given(DefaultSettings, But, () => PasswordIsShorterThan(12)).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_LowerCase() => await
        Given(DefaultSettings, But, PasswordHasNoLowerCase).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_UpperCase() => await
        Given(DefaultSettings, But, PasswordHasNoUpperCase).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_Digit() => await
        Given(DefaultSettings, But, PasswordHasNoDigit).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task Password_Should_Have_SpecialCharacter() => await
        Given(DefaultSettings, But, PasswordHasNoSpecialCharacter).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.PasswordMustBeContain));

    [Fact]
    public async Task UserName_Should_Be_Presented() => await
        Given(DefaultSettings, But, UserNameIsMissing).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.UserNameIsRequired));

    [Fact]
    public async Task Roles_Should_Be_Presented() => await
        Given(DefaultSettings, But, RolesIsMissing).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.AtLeastOneRoleRequired));

    [Fact]
    public async Task Roles_Should_Be_Registered() => await
        Given(DefaultSettings, But, RolesContainUnregistered).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.AtLeastOneRoleRequired));
}