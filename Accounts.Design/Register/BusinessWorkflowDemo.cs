using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class BusinessWorkflowDemo : BusinessWorkflowDemoDSL {
    [Fact]
    public Task ProductOwner_Should_Receive_A_Usable_Identity_When_The_Register_User_Story_Succeeds() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(ProductOwnerShouldReceiveAUsableIdentity);

    [Fact]
    public Task The_Register_User_Story_BusinessWorkflow_Should_Stop_Before_Later_BusinessWorkSteps_When_The_Request_Is_Invalid() =>
        Given(DefaultSettings, But, RequestHasAnyIssue).
        When(Run).
        Then(ProductOwnerShouldBeTold).
        Then(BusinessWorkflowShouldStopAfterValidation);

    [Fact]
    public Task The_Register_User_Story_BusinessWorkflow_Should_Stop_Before_Later_BusinessWorkSteps_When_A_Similar_Identity_Already_Exists() =>
        Given(DefaultSettings, But, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ProductOwnerShouldBeTold(Constants.AccountAlreadyExists)).
        Then(BusinessWorkflowShouldStopAfterPreventingDuplication);

    [Fact]
    public Task The_Register_User_Story_Should_Follow_The_Promised_BusinessWorkflow() =>
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(RegisterUserStoryShouldFollowThePromisedBusinessWorkflow);
}

public class BusinessWorkflowDemoDSL : FeatureDSL {
    protected void BusinessWorkflowShouldStopAfterValidation() =>
        ExecutedBusinessWorkSteps.ShouldBe([RegistrationWorkStep.Validation]);

    protected void BusinessWorkflowShouldStopAfterPreventingDuplication() =>
        ExecutedBusinessWorkSteps.ShouldBe([
            RegistrationWorkStep.Validation,
            RegistrationWorkStep.Normalization,
            RegistrationWorkStep.PreventDuplication,
        ]);

    protected void RegisterUserStoryShouldFollowThePromisedBusinessWorkflow() =>
        ExecutedBusinessWorkSteps.ShouldBe([
            RegistrationWorkStep.Validation,
            RegistrationWorkStep.Normalization,
            RegistrationWorkStep.PreventDuplication,
            RegistrationWorkStep.CreateIdentity,
            RegistrationWorkStep.SaveIdentity,
        ]);
}
