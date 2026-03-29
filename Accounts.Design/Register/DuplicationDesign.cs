using Accounts.Core.Domain;
using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class DuplicationDesign : FeatureDSL {
    [Fact]
    public Task Account_With_New_Email_Should_Be_Allowed() =>
        Given(DefaultSettings).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token)).
        Then(() => AccountRepository.Received(1).CreateAccount(Arg.Any<Account>(), Arg.Any<CancellationToken>()));

    [Fact]
    public Task Account_With_Same_Email_Should_Be_Not_Allowed() =>
        Given(DefaultSettings, But, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.AccountAlreadyExists)).
        Then(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token));

    [Fact]
    public Task RegisterAsync_PersistsAccountWithNormalizedCredentials() =>
        Given(DefaultSettings).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => Response.Email.ShouldBe("test-trader@bank.com")).
        Then(() => Response.UserName.ShouldBe(Request.UserName)).
        Then(() => Response.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true)).
        Then(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token)).
        Then(() => AccountRepository.Received(1).CreateAccount(Arg.Is<Account>(x => x.Email == "test-trader@bank.com" && x.UserName == Request.UserName), Token));
}
