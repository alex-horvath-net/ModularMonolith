using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class SaveDesign : FeatureDSL {
    [Fact]
    public Task New_Identity_Should_Be_Stored_For_Future_Work() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(NewIdentityShouldBeStored);

    [Fact]
    public Task Client_Should_Remain_In_Control_While_Registration_Is_Stored() =>
        Given(DefaultSettings, But, ClientProvidesWorkflowControl).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(ClientShouldRemainInControlWhileRegistrationIsStored);
}