using System.Globalization;
using Accounts.Core.Domain;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class CreateIdentityBusinessWorkStepDemo : FeatureDSL {
    [Fact]
    public Task The_CreateIdentity_BusinessWorkStep_Should_Protect_ProductOwner_Credentials() =>
        Given(DefaultSettings).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(CreateIdentityBusinessWorkStepShouldProtectProductOwnerCredentials);

    [Fact]
    public Task The_CreateIdentity_BusinessWorkStep_Should_Build_A_New_Identity_From_Normalized_ProductOwner_Data() =>
        Given(DefaultSettings, But, EmailIsNotNormalized, UserNameIsNotNormalized, RolesAreNotNormalized).
        When(Run).
        Then(RegisterUserStoryShouldBeAccepted).
        Then(CreateIdentityBusinessWorkStepShouldBuildANewIdentityFromNormalizedProductOwnerData);

    private void CreateIdentityBusinessWorkStepShouldProtectProductOwnerCredentials() {
        hasher.Received(1).Generate(Arg.Is<string>(password => password == "Ab!456789012"));
        accountRepository.Received(1).CreateAccount(Arg.Is<Account>(account => account.PasswordHash == "hashed-password"), token);
    }

    private void CreateIdentityBusinessWorkStepShouldBuildANewIdentityFromNormalizedProductOwnerData() =>
        accountRepository.Received(1).CreateAccount(Arg.Is<Account>(account =>
            account.Id != Guid.Empty &&
            account.Email == "test-trader@bank.com" &&
            account.UserName == "Test-Trader" &&
            account.PasswordHash == "hashed-password" &&
            account.Roles.Count == 1 &&
            account.Roles.Contains("Trader") &&
            account.CreatedAtUtc == DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture)), token);
}
