using Accounts.Core.Domain;
using Accounts.Register;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class PreventDuplicationBusinessWorkStepDemo : FeatureDSL {
    [Fact]
    public Task ProductOwner_Can_Start_The_Register_User_Story_When_No_Similar_Identity_Exists() =>
        Given(ProdLike).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(PreventDuplicationBusinessWorkStepShouldCheckExistingIdentity).
        Then(SaveIdentityBusinessWorkStepShouldStoreTheNewIdentity);

    [Fact]
    public Task ProductOwner_Can_Not_Start_The_Register_User_Story_Twice_With_The_Same_Email() =>
        Given(A, ProdLike, But, FastAndDeterministicDependencies, With, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.AccountAlreadyExists)).
        Then(PreventDuplicationBusinessWorkStepShouldCheckExistingIdentity);

    [Fact]
    public Task ProductOwner_Should_Receive_A_New_Stored_Identity_When_The_Register_User_Story_Is_Allowed() =>
        Given(ProdLike).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(ProductOwnerShouldReceiveAUsableIdentity).
        Then(PreventDuplicationBusinessWorkStepShouldCheckExistingIdentity).
        Then(SaveIdentityBusinessWorkStepShouldStoreTheNewIdentity);

    private void PreventDuplicationBusinessWorkStepShouldCheckExistingIdentity() =>
        accountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", token);

    private void SaveIdentityBusinessWorkStepShouldStoreTheNewIdentity() =>
        accountRepository.Received(1).CreateAccount(Arg.Any<Account>(), token);
}
