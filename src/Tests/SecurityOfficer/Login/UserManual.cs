using Tests.Shared;

namespace Tests.SecurityOfficer.Login;

[Collection(UserManualCollection.Name)]
public class UserManual(PlaywrightFixture trader) {

    [FactPlaywright]
    public async Task How_to_visit_login_page() {
        await trader.GoToPage("access/login");
        await trader.ExpectTextInElement("123", "UserContextId");
    }
}
