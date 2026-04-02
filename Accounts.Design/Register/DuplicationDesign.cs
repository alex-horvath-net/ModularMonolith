using Accounts.Core.Domain;
using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class DuplicationDesign : DuplicationDesignDSL {
    [Fact]
    public Task Client_Can_Register_When_No_Similar_Identity_Exists() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(ExistingIdentityShouldBeChecked).
        Then(NewIdentityShouldBeStored);

    [Fact]
    public Task Client_Can_Not_Register_Twice_With_The_Same_Email() =>
        Given(DefaultSettings, But, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.AccountAlreadyExists)).
        Then(ExistingIdentityShouldBeChecked);

    [Fact]
    public Task Client_Should_Receive_A_New_Stored_Identity_When_Registration_Is_Allowed() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(ClientShouldReceiveRegisteredIdentity).
        Then(ExistingIdentityShouldBeChecked).
        Then(NewIdentityShouldBeStored);
}

public class DuplicationDesignDSL : FeatureDSL {
    protected void ExistingIdentityShouldBeChecked() =>
        accountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", token);

    protected void NewIdentityShouldBeStored() =>
        accountRepository.Received(1).CreateAccount(Arg.Any<Account>(), token);
}
