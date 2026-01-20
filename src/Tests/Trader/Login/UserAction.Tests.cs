using Microsoft.Playwright;

namespace Tests.Trader.Login;

public class PlaywrightFixture : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; } = default!;
    public IBrowser Browser { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        Microsoft.Playwright.Program.Main(new[] { "install" });

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
    }
}

public class UserActionTests(PlaywrightFixture fixture) : IClassFixture<PlaywrightFixture>
{
    [Fact]
    public async Task TraderCanClickLoginButton()
    {
        var baseUrl = Environment.GetEnvironmentVariable("TRADINGPORTAL_BASE_URL") ?? "http://localhost:5001";

        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync(baseUrl);

        await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
    }
}
