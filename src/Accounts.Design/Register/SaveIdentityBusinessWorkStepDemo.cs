using Accounts.Core.Domain;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class SaveIdentityBusinessWorkStepDemo : FeatureDSL {
    [Fact]
    public Task The_SaveIdentity_BusinessWorkStep_Should_Store_The_New_Identity_For_Future_Work() =>
        Given(ProdLikeDependencies).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(SaveIdentityBusinessWorkStepShouldStoreTheNewIdentity);

    [Fact]
    public Task ProductOwner_Should_Remain_In_Control_While_The_Product_Stores_The_New_Identity() =>
        Given(ProdLikeDependencies, But, ProductOwnerProvidesBusinessWorkflowControl).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(ProductOwnerShouldRemainInControlWhileTheProductStoresTheNewIdentity);

    private void SaveIdentityBusinessWorkStepShouldStoreTheNewIdentity() =>
        accountRepository.Received(1).CreateAccount(Arg.Any<Account>(), token);

    private void ProductOwnerShouldRemainInControlWhileTheProductStoresTheNewIdentity() {
        token.ShouldNotBe(CancellationToken.None);
        token.IsCancellationRequested.ShouldBeTrue();
        accountRepository.Received(1).CreateAccount(Arg.Any<Account>(), token);
    }
}
