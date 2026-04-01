using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class CreateDesign : FeatureDSL {
    [Fact]
    public Task New_Identity_Should_Protect_Client_Credentials() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(NewIdentityShouldProtectCredentials);

    [Fact]
    public Task New_Identity_Should_Be_Built_From_Normalized_Client_Data() =>
        Given(DefaultSettings, But, EmailIsNotNormalized, UserNameIsNotNormalized, RolesAreNotNormalized).
        When(Run).
        Then(RegistrationShouldBeAccepted).
        Then(NewIdentityShouldBeBuiltFromNormalizedClientData);
}
