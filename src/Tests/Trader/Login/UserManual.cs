using Tests.Shared;

namespace Tests.Trader.Login;

[Collection(UserManualCollection.Name)]
public class UserManual(TradingPortalPlaywrigh trader) {

    [Fact]
    public async Task TraderSeesUnderConstructionPopupOnLoginClick() {
        await trader.GoToPage("access/login");
        await trader.ClickOnButton("Login");
        await trader.ShouldBe("marker", "handled");
    }
}
