using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class OrchestratorDesign : OrchestratorDesignDSL {
    [Fact]
    public Task Client_Should_Receive_A_Usable_Identity_When_Registration_Succeeds() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(ClientShouldReceiveRegisteredIdentity);

    [Fact]
    public Task Registration_Should_Stop_Before_Later_Work_When_Request_Is_Invalid() =>
        Given(DefaultSettings, But, RequestHasAnyIssue).
        When(Run).
        Then(ClientShouldBeTold).
        Then(WorkflowShouldStopAfterValidation);

    [Fact]
    public Task Registration_Should_Stop_Before_Later_Work_When_A_Similar_Identity_Already_Exists() =>
        Given(DefaultSettings, But, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.AccountAlreadyExists)).
        Then(WorkflowShouldStopAfterPreventingDuplication);

    [Fact]
    public Task Registration_Should_Follow_The_Promised_Business_Workflow() =>
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(RegistrationShouldFollowThePromisedWorkflow);
}

public class OrchestratorDesignDSL : FeatureDSL {
    protected void WorkflowShouldStopAfterValidation() =>
        ExecutedWorkSteps.ShouldBe([RegistrationWorkStep.Validation]);

    protected void WorkflowShouldStopAfterPreventingDuplication() =>
        ExecutedWorkSteps.ShouldBe([
            RegistrationWorkStep.Validation,
            RegistrationWorkStep.Normalization,
            RegistrationWorkStep.PreventDuplication,
        ]);

    protected void RegistrationShouldFollowThePromisedWorkflow() =>
        ExecutedWorkSteps.ShouldBe([
            RegistrationWorkStep.Validation,
            RegistrationWorkStep.Normalization,
            RegistrationWorkStep.PreventDuplication,
            RegistrationWorkStep.CreateIdentity,
            RegistrationWorkStep.SaveIdentity,
        ]);
}