using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class DuplicationDesign : FeatureDSL {
    [Fact]
    public async Task Account_With_Same_Email_Should_Be_Not_Allowed() => await
        Given(AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ShouldFailWith(Constants.AccountAlreadyExists)).
        Then(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token));

    [Fact]
    public async Task Account_With_New_Email_Should_Be_Allowed() => await
        Given(DefaultSettings).
        When(Run).
        Then(() => AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token)).
        Then(() => AccountRepository.Received(1).CreateAccount(Arg.Any<Core.Domain.Account>(), Arg.Any<CancellationToken>()));

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials() => await
        Given(DefaultSettings).
        When(Run).
        Then(() => {
            Response.Email.ShouldBe("test-trader@bank.com");
            Response.UserName.ShouldBe(Request.UserName);
            Response.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true);

            AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token);
            AccountRepository.Received(1).CreateAccount(
                Arg.Is<Core.Domain.Account>(account => account.Email == "test-trader@bank.com" && account.UserName == Request.UserName),
                Token);
        });

}
