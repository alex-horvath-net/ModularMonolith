using System.Globalization;
using Accounts.Core.Domain;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class CreateDesign : CreateDesignDSL {
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

public class CreateDesignDSL : FeatureDSL {
    protected void NewIdentityShouldProtectCredentials() {
        hasher.Received(1).Generate(CurrentRequest.Password);
        accountRepository.Received(1).CreateAccount(Arg.Is<Account>(account => account.PasswordHash == "hashed-password"), token);
    }

    protected void NewIdentityShouldBeBuiltFromNormalizedClientData() =>
        accountRepository.Received(1).CreateAccount(Arg.Is<Account>(account =>
            account.Id != Guid.Empty &&
            account.Email == "test-trader@bank.com" &&
            account.UserName == "Test-Trader" &&
            account.PasswordHash == "hashed-password" &&
            account.Roles.Count == 1 &&
            account.Roles.Contains("Trader") &&
            account.CreatedAtUtc == DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture)), token);
}
