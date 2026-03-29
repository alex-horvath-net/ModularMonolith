using Accounts.Core.Domain;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class SaveDesign : FeatureDSL {
    [Fact]
    public Task Created_Account_Should_Be_Saved() =>
        Given(DefaultSettings).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => AccountRepository.Received(1).CreateAccount(Arg.Any<Account>(), Token));

    [Fact]
    public Task Created_Account_Should_Be_Saved_With_Provided_Token() =>
        Given(DefaultSettings).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => AccountRepository.Received(1).CreateAccount(Arg.Any<Account>(), Token));
}