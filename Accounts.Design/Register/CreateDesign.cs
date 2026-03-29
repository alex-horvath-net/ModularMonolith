using System.Globalization;
using Accounts.Core.Domain;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class CreateDesign : FeatureDSL {
    [Fact]
    public Task Password_Should_Be_Hashed_When_Account_Is_Created() =>
        Given(DefaultSettings).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => Hasher.Received(1).Generate(Request.Password)).
        Then(() => AccountRepository.Received(1).CreateAccount(Arg.Is<Account>(account => account.PasswordHash == "hashed-password"), Token));

    [Fact]
    public Task Account_Should_Be_Created_From_Normalized_Request() =>
        Given(DefaultSettings, But, EmailIsNotNormalized, UserNameIsNotNormalized, RolesAreNotNormalized).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => AccountRepository.Received(1).CreateAccount(Arg.Is<Account>(account =>
            account.Id != Guid.Empty &&
            account.Email == "test-trader@bank.com" &&
            account.UserName == "Test-Trader" &&
            account.PasswordHash == "hashed-password" &&
            account.Roles.Count == 1 &&
            account.Roles.Contains("Trader") &&
            account.CreatedAtUtc == DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture)), Token));
}
