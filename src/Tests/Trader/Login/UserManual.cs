using Tests.Shared;

namespace Tests.Trader.Login;

[Collection(UserManualCollection.Name)]
public class UserManual(PlaywrightFixture trader) {

    [Fact]
    public async Task How_to_login() {
        await trader.GoToPage("access/login");
        await trader.ClickOnButton("Login");
        await trader.ExpectTextInElement("handled", "marker");
    }
} 
