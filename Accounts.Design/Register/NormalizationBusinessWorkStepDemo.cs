using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class NormalizationBusinessWorkStepDemo : NormalizationBusinessWorkStepDemoDSL {
    [Fact]
    public Task ProductOwner_Should_Receive_A_Normalized_Email_From_The_Product() =>
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeEmail("test-trader@bank.com"));

    [Fact]
    public Task ProductOwner_Should_Receive_A_Normalized_UserName_From_The_Product() =>
        Given(DefaultSettings, But, UserNameIsNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeUserName("Test-Trader"));

    [Fact]
    public Task ProductOwner_Should_Receive_Normalized_Roles_From_The_Product() =>
        Given(DefaultSettings, But, RolesAreNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeRoles("Trader"));
}

public class NormalizationBusinessWorkStepDemoDSL : FeatureDSL {
}
