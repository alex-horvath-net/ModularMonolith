namespace Accounts.Design.Register;

public class NormalizationDesign : FeatureDSL {
    [Fact]
    public async Task Email_Should_Be_Normalized() => await
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(() => Response.Email.ShouldBe("test-trader@bank.com"));

    [Fact]
    public async Task UserName_Should_Be_Normalized() => await
        Given(UserNameIsNotNormalized).
        When(Run).
        Then(() => Response.UserName.ShouldBe("Test-Trader"));

    [Fact]
    public async Task Roles_Should_Be_Normalized() => await
        Given(RolesAreNotNormalized).
        When(Run).
        Then(() => Response.Roles.ShouldBe(["Trader"]));
}
