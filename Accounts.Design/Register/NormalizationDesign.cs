namespace Accounts.Design.Register;

public sealed class NormalizationDesign : FeatureDSL {
    [Fact]
    public async Task Email_Should_Be_Normalized() => await
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(() => SUT.ShouldNotThrowAsync()).
        Then(() => Response.Email.ShouldBe("test-trader@bank.com"));

    [Fact]
    public async Task UserName_Should_Be_Normalized() => await
        Given(DefaultSettings, But, UserNameIsNotNormalized).
        When(Run).
        Then(() => SUT.ShouldNotThrowAsync()).
        Then(() => Response.UserName.ShouldBe("Test-Trader"));

    [Fact]
    public async Task Roles_Should_Be_Normalized() => await
        Given(DefaultSettings, But, RolesAreNotNormalized).
        When(Run).
        Then(() => SUT.ShouldNotThrowAsync()).
        Then(() => Response.Roles.ShouldBe(["Trader"]));
}
