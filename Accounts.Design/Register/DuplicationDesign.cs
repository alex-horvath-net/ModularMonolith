using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class DuplicationDesign : FeatureDSL {
    internal override UserStory Unit() => new(AccountantRepository, Hasher, Clock);
    internal override Task<Response> Call(UserStory userStory) => userStory.Register(Request, Token);
    internal override string WorkStep() => "Duplication";

    [Fact]
    public Task Account_With_Same_Email_Should_Be_Not_Allowed() =>
        Given.AccountAlreadyExistsWithSimilarEmail().
        When.Register().
        Then.ShouldFailWith(Constants.AccountAlreadyExists, dsl =>
            dsl.AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", dsl.CurrentToken));

    [Fact]
    public Task Account_With_New_Email_Should_Be_Allowed() =>
        When.Register().
        Then.ShouldSucceedWith((dsl, _) => {
            dsl.AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", dsl.CurrentToken);
            dsl.AccountRepository.Received(1).CreateAccount(Arg.Any<Accounts.Core.Domain.Account>(), Arg.Any<CancellationToken>());
        });

    [Fact]
    public Task RegisterAsync_PersistsAccountWithNormalizedCredentials() =>
        When.Register().
        Then.ShouldSucceedWith((dsl, result) => {
            result.Email.ShouldBe("test-trader@bank.com");
            result.UserName.ShouldBe(dsl.CurrentRequest.UserName);
            result.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true);

            dsl.AccountRepository.Received(1).FindAccountByEmail("test-trader@bank.com", dsl.CurrentToken);
            dsl.AccountRepository.Received(1).CreateAccount(
                Arg.Is<Accounts.Core.Domain.Account>(account =>
                    account.Email == "test-trader@bank.com" &&
                    account.UserName == dsl.CurrentRequest.UserName),
                dsl.CurrentToken);
        });

}
