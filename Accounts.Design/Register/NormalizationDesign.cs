namespace Accounts.Design.Register;

public class NormalizationDesign : Fixtrure {

    [Fact]
    public async Task Email_Should_Be_Normalized() {
        var result = await WhenEmailIsNotNormalized().SUT();
        result.Email.ShouldBe("test-trader@bank.com");
    }

    [Fact]
    public async Task UserName_Should_Be_Normalized() {
        var result = await WhenUserNameIsNormalized().SUT();
        result.UserName.ShouldBe("Test-Trader");
    }

    [Fact]
    public async Task Roles_Should_Be_Normalized() {
        var result = await WhenRolesAreNotNormalized().SUT();
        result.Roles.ShouldBe(["Trader"]);
    }

    protected NormalizationDesign WhenEmailIsNotNormalized() {
        var request = RequestFactory();
        RequestFactory = () => request with { Email = " Test-Trader@Bank.Com  " };
        return this;
    }

    protected NormalizationDesign WhenUserNameIsNormalized() {
        var request = RequestFactory();
        RequestFactory = () => request with { UserName = " Test-Trader " };
        return this;
    }
    protected NormalizationDesign WhenRolesAreNotNormalized() {
        var request = RequestFactory();
        RequestFactory = () => request with { Roles = [null, "", " ", "Trader", " TradeR "] };
        return this;
    }
}
