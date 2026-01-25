using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using static System.Net.Mime.MediaTypeNames;

namespace Tests.Shared;

public class PlaywrightFixture : IAsyncLifetime {
    public static readonly Lazy<bool> Skip = new(() => {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetValue<bool>("Playwright:Skip");
    });

    private Process? portalProcess;
    private IPlaywright playwright = null!;
    private IBrowser browser = null!;
    private IBrowserContext? browserContext;
    private IPage page = null!;
    private readonly List<IBrowserContext> browserContexts = [];
    private string baseUrl = null!;

    public async Task InitializeAsync() {
        if (Skip.Value) {
            return;
        }

        //Microsoft.Playwright.Program.Main(["install"]);

        playwright = await Playwright.CreateAsync();
        var options = new BrowserTypeLaunchOptions();
        options.Headless = true;
        browser = await playwright.Chromium.LaunchAsync(options);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        baseUrl = configuration["Playwright:BaseUrl"];
        StartTradingPortal();
        await WaitForPortal(20_000);
    }

    public async Task GoToPage(string relativeUrl, int timeoutMs = 5000) {
        if (browserContext is not null) {
            await browserContext.CloseAsync();
        }

        browserContext = await browser.NewContextAsync();
        browserContexts.Add(browserContext);
        this.page = await browserContext.NewPageAsync();
        var absoluteUrl = new Uri(new Uri(baseUrl), relativeUrl).ToString();
        await this.page.GotoAsync(absoluteUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeoutMs });
    }

    public async Task ClickOnButton(string name, int timeoutMs = 5000) {
        var button = page.GetByRole(AriaRole.Button, new() { Name = name });
        await button.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeoutMs });
        await button.ClickAsync(new LocatorClickOptions { Timeout = timeoutMs });
    }

    public Task ExpectElementNotEmpty(string id, int timeoutMs = 5000) => Assertions
        .Expect(page.Locator($"#{id}"))
        .ToHaveTextAsync( 
            new Regex(@".+"),
            new LocatorAssertionsToHaveTextOptions { Timeout = timeoutMs }
        );

    public Task ExpectTextInElement(string text, string id, int timeoutMs = 5000) => Assertions
        .Expect(page.Locator($"#{id}"))
        .ToHaveTextAsync(text, new LocatorAssertionsToHaveTextOptions { Timeout = timeoutMs });

    public async Task DisposeAsync() {
        if (Skip.Value) {
            return;
        }

        foreach (var context in browserContexts) {
            await context.CloseAsync();
        }

        if (portalProcess is not null && !portalProcess.HasExited) {
            try {
                portalProcess.Kill(entireProcessTree: true);
            } catch {
                // ignore best-effort shutdown
            }
        }
        if (browser is not null) {
            await browser.CloseAsync();
        }

        playwright?.Dispose();
    }

    private void StartTradingPortal() {
        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var projectPath = Path.Combine(solutionRoot, "TradingPortal", "TradingPortal.csproj");

        var startInfo = new ProcessStartInfo {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --urls {baseUrl} --no-sandbox, --disable-dev-shm-usage",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = solutionRoot
        };

        portalProcess = Process.Start(startInfo);
    }

    private async Task WaitForPortal(int timeoutMs = 20_000) {
        using var client = new HttpClient();
        var timeout = TimeSpan.FromMilliseconds(timeoutMs);
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout) {
            try {
                using var response = await client.GetAsync(baseUrl);
                if (response.IsSuccessStatusCode) {
                    return;
                }
            } catch {
                // retry until timeout
            }

            await Task.Delay(500);
        }

        throw new InvalidOperationException($"TradingPortal did not start at {baseUrl} within {timeout.TotalSeconds} seconds.");
    }

}
