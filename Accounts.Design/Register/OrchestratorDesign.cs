using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class OrchestratorDesign : FeatureDSL {
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
        Then(StopBeforeDeduplication).
        Then(RegistrationShouldStopBeforeProtectingCredentials).
        Then(RegistrationShouldStopBeforeStoringNewIdentity);

    [Fact]
    public Task Registration_Should_Stop_Before_Later_Work_When_A_Similar_Identity_Already_Exists() =>
        Given(DefaultSettings, But, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ClientShouldBeTold(Constants.AccountAlreadyExists)).
        Then(RegistrationShouldStopBeforeProtectingCredentials).
        Then(RegistrationShouldStopBeforeStoringNewIdentity);

    [Fact]
    public Task Registration_Should_Follow_The_Promised_Business_Workflow() =>
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(RegistrationShouldFollowThePromisedWorkflow);
}
