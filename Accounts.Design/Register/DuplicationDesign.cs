using Accounts.Core.Domain;
using Accounts.Core.Infrastructure;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class DuplicationDesign : Fixtrure {

    [Fact]
    public async Task Account_With_Same_Email_Should_Be_Not_Allowed() {
        var exception = WhenAccountAlreadyExistsWithSimilarEmail().SUT.ShouldThrow<InvalidOperationException>();

        exception.Message.ShouldBe(Constants.AccountAlreadyExists);
        await AccountantRepository.Received(1).FindAccountByEmail("test-trader@bank.com", Token);
        await AccountantRepository.DidNotReceive().CreateAccount(Arg.Any<Account>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Account_With_New_Email_Should_Be_Allowed() {
        SUT.ShouldNotThrow();

        await AccountantRepository.Received().CreateAccount(Arg.Any<Account>(), Arg.Any<CancellationToken>());
    }

    protected DuplicationDesign WhenAccountAlreadyExistsWithSimilarEmail() {
        var existingAccount = new Account(
            Guid.NewGuid(),
            EmailFactory(),
            UserNameFactory(),
            PasswordFactory(),
            RolesFactory().ToHashSet(),
            IsLocked: false,
            CreatedAtUtc: DateTime.Parse("2024-01-01T00:00:00Z", System.Globalization.CultureInfo.InvariantCulture));

        var mock = AccountRepositoryFactory();
        mock.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(existingAccount);

        AccountRepositoryFactory = () => mock;

        return this;
    }

}
