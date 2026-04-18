using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class NormalizationBusinessWorkStepDemo : DSL {
    [Fact]
    public Task ProductOwner_Should_Receive_A_Normalized_Email_From_The_Product() =>
        Given(ProdLike, But, FastAndDeterministicDependencies, With, EmailIsNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeEmail("test-trader@bank.com"));

    [Fact]
    public Task ProductOwner_Should_Receive_A_Normalized_UserName_From_The_Product() =>
        Given(ProdLike, But, UserNameIsNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeUserName("Test-Trader"));

    [Fact]
    public Task ProductOwner_Should_Receive_Normalized_Roles_From_The_Product() =>
        Given(ProdLike, But, RolesAreNotNormalized).
        When(Run).
        Then(() => ProductOwnerShouldSeeRoles("Trader"));
}
