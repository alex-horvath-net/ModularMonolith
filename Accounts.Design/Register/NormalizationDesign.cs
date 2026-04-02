
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class NormalizationDesign : NormalizationDesignDSL {
    [Fact]
    public Task Client_Should_Receive_A_Normalized_Email() =>
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(() => ClientShouldSeeEmail("test-trader@bank.com"));

    [Fact]
    public Task Client_Should_Receive_A_Normalized_UserName() =>
        Given(DefaultSettings, But, UserNameIsNotNormalized).
        When(Run).
        Then(() => ClientShouldSeeUserName("Test-Trader"));

    [Fact]
    public Task Client_Should_Receive_Normalized_Roles() =>
        Given(DefaultSettings, But, RolesAreNotNormalized).
        When(Run).
        Then(() => ClientShouldSeeRoles("Trader"));
}

public class NormalizationDesignDSL : FeatureDSL {
}
