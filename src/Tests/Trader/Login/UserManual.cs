using Microsoft.Playwright;
using Tests.Shared;

namespace Tests.Trader.Login;

public class UserManual(TradingPortalPlaywrigh fixture) : IClassFixture<TradingPortalPlaywrigh>
{
    [Fact]
    public async Task TraderCanClickLoginButton()
    {
        var (page, baseUrl) = await fixture.GetPage();

        await page.GotoAsync($"{baseUrl}/access/login");

        await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
    }
}
