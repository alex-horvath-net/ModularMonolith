namespace Accounts.UserManual.Login;

[Collection(UserManualICollectionFixture.Name)]
public class UserManual(PlaywrightFixture trader) {

    private const string DefaultEmail = "aladar.horvath@outlook.com";
    private const string DefaultPassword = "Sup3r$ecretPwd!";

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

        await trader.ExpectElementEmpty("UserName2");
    }

    [FactPlaywright]
    public async Task How_to_login() {
        await trader.GoToPage("access/login");
        await trader.FillInput("Email", DefaultEmail);
        await trader.FillInput("Password", DefaultPassword);

        await trader.ClickOnButton("Login");

        await trader.ExpectElementNotEmpty("ApplicationName");
        await trader.ExpectElementNotEmpty("ApplicationVersion");

        await trader.ExpectElementNotEmpty("VisitorId");
        await trader.ExpectElementNotEmpty("VisitStartedAt");
        await trader.ExpectElementEmpty("AuthenticationId");
        await trader.ExpectElementEmpty("UserName");

        await trader.ExpectElementNotEmpty("Role1");

        await trader.ExpectTextInElement("Aladar Horvath", "UserName2");
    }
}
