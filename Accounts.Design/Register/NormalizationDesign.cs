
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class NormalizationDesign : FeatureDSL {
    [Fact]
    public Task Email_Should_Be_Normalized() =>
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(() => Response.Email.ShouldBe("test-trader@bank.com"));

    [Fact]
    public Task UserName_Should_Be_Normalized() =>
        Given(DefaultSettings, But, UserNameIsNotNormalized).
        When(Run).
        Then(() => Response.UserName.ShouldBe("Test-Trader"));

    [Fact]
    public Task Roles_Should_Be_Normalized() =>
        Given(DefaultSettings, But, RolesAreNotNormalized).
        When(Run).
        Then(() => Response.Roles.ShouldBe(["Trader"]));
}
