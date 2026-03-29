using Accounts.Core.Domain;
using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class DuplicationDesign : FeatureDSL {
    [Fact]
    public async Task Account_With_Same_Email_Should_Be_Not_Allowed() => await
        Given(DefaultSettings, But, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Next(() => ShouldThrow<InvalidOperationException>(Constants.AccountAlreadyExists)).
        Next(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token));

    [Fact]
    public async Task Account_With_New_Email_Should_Be_Allowed() => await
        Given(DefaultSettings).
        When(Run).
        Next(ShouldNotThrowException).
        Next(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token)).
        Next(() => AccountRepository.Received(1).CreateAccount(Arg.Any<Account>(), Arg.Any<CancellationToken>()));

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials() => await
        Given(DefaultSettings).
        When(Run).
        Next(ShouldNotThrowException).
        Next(() => Response.Email.ShouldBe("test-trader@bank.com")).
        Next(() => Response.UserName.ShouldBe(Request.UserName)).
        Next(() => Response.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true)).
        Next(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token)).
        Next(() => AccountRepository.Received(1).CreateAccount(Arg.Is<Account>(x => x.Email == "test-trader@bank.com" && x.UserName == Request.UserName), Token));
}
