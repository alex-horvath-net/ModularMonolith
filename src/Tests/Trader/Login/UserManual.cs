using Tests.Shared;

namespace Tests.Trader.Login;

[Collection(UserManualCollection.Name)]
public class UserManual(TradingPortalPlaywrigh visitor) {
    [Fact]
    public async Task TraderCanClickLoginButton() {
        await visitor.GoToPage("access/login");
        await visitor.ClickOnButton("Login");
    }


    [Fact]
    public async Task TraderSeesUnderConstructionPopupOnLoginClick() {
        await visitor.GoToPage("access/login");
        await visitor.ClickOnButton("Login");
        await visitor.WaitForTextAsync("Trader-Login-UserAction", timeoutMs: 2000);
    }
}
