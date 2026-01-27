using Tests.Shared;

namespace Tests.SecurityOfficer.Login;

[Collection(UserManualCollection.Name)]
public class UserManual(PlaywrightFixture trader) {

 

    [FactPlaywright]
    public async Task How_to_visit_login_page() {
        await trader.GoToPage("access/login");

        await trader.ExpectElementNotEmpty("ApplicationName");
        await trader.ExpectElementNotEmpty("ApplicationVersion");

        await trader.ExpectElementNotEmpty("VisitorId");
        await trader.ExpectElementNotEmpty("VisitStartedAt");
        await trader.ExpectElementEmpty("AuthenticationId");
        await trader.ExpectElementEmpty("UserName");

        await trader.ExpectElementNotEmpty("Role1");

        await trader.ExpectElementEmpty("IsUserStoryEnabled");
    }

    [FactPlaywright]
    public async Task How_to_login() {
        await trader.GoToPage("access/login");
        await trader.FillInput("Email", "aladar.horvath@outlook.com");
        await trader.FillInput("Password", "P@ssw0rd!");

        await trader.ClickOnButton("Login");

        await trader.ExpectElementNotEmpty("ApplicationName");
        await trader.ExpectElementNotEmpty("ApplicationVersion");

        await trader.ExpectElementNotEmpty("VisitorId");
        await trader.ExpectElementNotEmpty("VisitStartedAt");
        await trader.ExpectElementEmpty("AuthenticationId");
        await trader.ExpectElementEmpty("UserName");

        await trader.ExpectElementNotEmpty("Role1");

        await trader.ExpectTextInElement("True", "IsUserStoryEnabled");
    }
}
