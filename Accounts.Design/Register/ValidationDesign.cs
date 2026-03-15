using Accounts.Core.Domain;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class ValidationDesign : Fixtrure {

    //Validate
    [Fact]
    public void Request_Should_Be_Presented() =>
        WhenRequestIsMissing().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.RequestCanNotBeNell);

    [Fact]
    public void Email_Should_Be_Presented() =>
        WhenEmailIsMissing().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.EmailIsRequired);

    [Fact]
    public void Password_Should_Be_Presented() =>
        WhenPasswordIsMissing().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.PasswordMutBeContain);

    [Fact]
    public void Password_Should_Be_At_Least_12_Characters() =>
       WhenPasswordIsTooShort().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.PasswordMutBeContain);

    [Fact]
    public void Password_Should_Have_Atleast_1_Upper_Case_Character() =>
       WhenPasswordHasNoUpperCase().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.PasswordMutBeContain);

    [Fact]
    public void Password_Should_Have_Atleast_1_Lower_Case_Character() =>
        WhenPasswordHasNoLowerCase().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.PasswordMutBeContain);

    [Fact]
    public void Password_Should_Have_Atleast_1_Digit_Character() =>
         WhenPasswordHasNoDigit().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.PasswordMutBeContain);

    [Fact]
    public void Password_Should_Have_Atleast_1_Special_Character() =>
         WhenPasswordHasNoSpecialCharacter().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.PasswordMutBeContain);

    [Fact]
    public void UserName_Should_Be_Presented() =>
       WhenUserNameIsMissing().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.UserNameIsRequired);

    [Fact]
    public void Role_Should_Be_Presented() =>
        WhenRoleIsMissing().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.AtLeastOneRoleRequired);

    [Fact]
    public void Roles_Should_Be_Clean() =>
        WhenTherIsNoCleanRole().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.AtLeastOneRoleRequired);

    [Fact]
    public void Role_Should_Be_Registered() =>
        WhenTherIsNoRegisteredRole().SUT.ShouldThrow<InvalidOperationException>().Message.ShouldBe(Constants.AtLeastOneRoleRequired);

    [Fact]
    public async Task RegisterAsync_PersistsAccountWithNormalizedCredentials() {
        var result = await SUT();

        result.Email.ShouldBe(Request.Email);
        result.UserName.ShouldBe(Request.UserName);
        result.Roles.ShouldBe(["Trader", "RiskManager"], ignoreOrder: true);

        await AccountantRepository.Received(1).FindAccountByEmail(Request.Email, Token);
        await AccountantRepository.Received(1).CreateAccount(
            Arg.Is<Account>(account =>
                account.Email == Request.Email &&
                account.UserName == Request.UserName
                //account.Roles.SetEquals(["Trader", "RiskManager"]) &&
                //account.PasswordHash == "hashed-password"
                ),
            Token);
    }

    protected ValidationDesign WhenTherIsNoRegisteredRole() {
        RolesFactory = () => ["Trader", "UnRegisteredRole"];
        return this;
    }

    protected ValidationDesign WhenTherIsNoCleanRole() {
        RolesFactory = () => [null, "", " "];
        return this;
    }

    protected ValidationDesign WhenRoleIsMissing() {
        RolesFactory = () => null;
        return this;
    }

    protected ValidationDesign WhenUserNameIsMissing() {
        UserNameFactory = () => null!;
        return this;
    }

    protected ValidationDesign WhenPasswordHasNoSpecialCharacter() {
        PasswordFactory = () => "Ab123456789012";
        return this;
    }

    protected ValidationDesign WhenPasswordHasNoDigit() {
        PasswordFactory = () => "Aabbbbbbbbbb";
        return this;
    }

    protected ValidationDesign WhenPasswordHasNoLowerCase() {
        PasswordFactory = () => "A123456789012";
        return this;
    }

    protected ValidationDesign WhenPasswordHasNoUpperCase() {
        PasswordFactory = () => "a123456789012";
        return this;
    }

    protected ValidationDesign WhenPasswordIsTooShort() {
        PasswordFactory = () => "1234";
        return this;
    }

    protected ValidationDesign WhenPasswordIsMissing() {
        PasswordFactory = () => null;
        return this;
    }

    protected ValidationDesign WhenEmailIsMissing() {
        EmailFactory = () => null!;
        return this;
    }

    protected ValidationDesign WhenRequestIsMissing() {
        RequestFactory = () => null!;
        return this;
    }
}
