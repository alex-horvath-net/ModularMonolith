using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class NormalizationBusinessWorkStepDemo : FeatureDSL {
    [Fact]
    public Task ProductOwner_Should_Receive_A_Normalized_Email_From_The_Product() =>
        Given(ProdLikeDependencies, But, EmailIsNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeEmail("test-trader@bank.com"));

    [Fact]
    public Task ProductOwner_Should_Receive_A_Normalized_UserName_From_The_Product() =>
        Given(ProdLikeDependencies, But, UserNameIsNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeUserName("Test-Trader"));

    [Fact]
    public Task ProductOwner_Should_Receive_Normalized_Roles_From_The_Product() =>
        Given(ProdLikeDependencies, But, RolesAreNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeRoles("Trader"));
}
