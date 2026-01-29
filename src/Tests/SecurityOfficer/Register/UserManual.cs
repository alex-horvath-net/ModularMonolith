using System;
using Tests.Shared;

namespace Tests.SecurityOfficer.Register;

[Collection(UserManualCollection.Name)]
public class UserManual(PlaywrightFixture trader)
{
    [FactPlaywright]
    public async Task How_to_visit_register_page()
    {
        await trader.GoToPage("access/register");

        await trader.ExpectElementEmpty("RegistrationSuccess");
        await trader.ExpectElementEmpty("RegistrationError");
    }

    [FactPlaywright]
    public async Task How_to_register()
    {
        await trader.GoToPage("access/register");

        var email = $"trader+{Guid.NewGuid():N}@bank.com";
        const string password = "Sup3r$ecretPwd!";

        await trader.FillInput("Email", email);
        await trader.FillInput("UserName", "Jane Trader");
        await trader.FillInput("Password", password);
        await trader.FillInput("ConfirmPassword", password);
        await trader.SetCheckbox("role-Trader", true);

        await trader.ClickOnButton("Create Identity");

        await trader.ExpectTextInElement($"Registration complete for {email}.", "RegistrationSuccess");
        await trader.ExpectElementEmpty("RegistrationError");
    }
}
