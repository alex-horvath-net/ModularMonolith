using Accounts.Core.Domain;
using Accounts.Register.UserStory;
using Core.Domain.Tasks;

namespace Accounts.Design.Register;

public sealed class OrchestratorDesign : FeatureDSL {
    [Fact]
    public Task Client_Should_Get_Feedback_When_Registration_Workflow_Succeeds() =>
        Given(DefaultSettings).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => Response.AccountId.ShouldNotBe(Guid.Empty)).
        Then(() => Response.Email.ShouldBe("test-trader@bank.com")).
        Then(() => Response.UserName.ShouldBe("Test-Trader")).
        Then(() => Response.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true));

    [Fact]
    public Task Workflow_Should_Stop_When_Validation_Fails() =>
        Given(DefaultSettings, But, EmailIsMissing).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.EmailIsRequired)).
        Then(() => AccountRepository.DidNotReceiveWithAnyArgs().FindAccountByEmail(default!, default)).
        Then(() => Hasher.DidNotReceiveWithAnyArgs().Generate(default!)).
        Then(() => AccountRepository.DidNotReceiveWithAnyArgs().CreateAccount(default!, default));

    [Fact]
    public Task Workflow_Should_Stop_When_Duplication_Fails() =>
        Given(DefaultSettings, But, AccountAlreadyExistsWithSimilarEmail).
        When(Run).
        Then(() => ShouldThrow<InvalidOperationException>(Constants.AccountAlreadyExists)).
        Then(() => Hasher.DidNotReceiveWithAnyArgs().Generate(default!)).
        Then(() => AccountRepository.DidNotReceiveWithAnyArgs().CreateAccount(default!, default));

    [Fact]
    public Task Business_WorkSteps_Should_Be_Orchestrated_In_Order() =>
        Given(DefaultSettings, But, EmailIsNotNormalized).
        When(Run).
        Then(ShouldNotThrowException).
        Then(() => Received.InOrder(() => {
            AccountRepository.FindAccountByEmail("test-trader@bank.com", Token);
            Hasher.Generate(Request.Password);
            AccountRepository.CreateAccount(Arg.Any<Account>(), Token);
        }));
}
