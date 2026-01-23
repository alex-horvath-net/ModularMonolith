using Tests.Shared;

namespace Tests.Trader.Login;

[Collection(UserManualCollection.Name)]
public class UserManual(PlaywrightFixture trader) {

    [Fact]
    public async Task TraderSeesUnderConstructionPopupOnLoginClick() {
        await trader.GoToPage("access/login");
        await trader.ClickOnButton("Login");
        await trader.ExpectTextInElement("handled", "marker");
    }
}
