using System.Data.Common;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace Tests.TestFrame;

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
    private string baseUrl = string.Empty;

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

        baseUrl = configuration["Playwright:BaseUrl"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(baseUrl)) {
            throw new InvalidOperationException("Playwright:BaseUrl is not configured.");
        }
        StartTradingPortal();
        await WaitForPortal(60_000);
    }

    public async Task GoToPage(string relativeUrl, int timeoutMs = 5000) {
        if (browserContext is not null) {
            await browserContext.CloseAsync();
        }

        browserContext = await browser.NewContextAsync();
        browserContexts.Add(browserContext);
        page = await browserContext.NewPageAsync();
        var absoluteUrl = new Uri(new Uri(baseUrl), relativeUrl).ToString();
        await page.GotoAsync(absoluteUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeoutMs });
    }

    public async Task ClickOnButton(string name, int timeoutMs = 5000) {
        var button = page.GetByRole(AriaRole.Button, new() { Name = name });
        await button.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeoutMs });
        await button.ClickAsync(new LocatorClickOptions { Timeout = timeoutMs });
    }

    public async Task ExpectElementNotEmpty(string id, int timeoutMs = 10000) {
        var locator = page.Locator($"#{id}");
        await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = timeoutMs });
        await Assertions.Expect(locator)
            .ToHaveTextAsync(
                new Regex(@".+"),
                new LocatorAssertionsToHaveTextOptions { Timeout = timeoutMs }
            );
    }

    public async Task ExpectElementEmpty(string id, int timeoutMs = 10000) {
        var locator = page.Locator($"#{id}");
        await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = timeoutMs });
        await Assertions.Expect(locator)
            .ToHaveTextAsync(
                new Regex(@"^\s*$"),
                new LocatorAssertionsToHaveTextOptions { Timeout = timeoutMs }
            );
    }

    public async Task ExpectTextInElement(string text, string id, int timeoutMs = 10000) {
        var locator = page.Locator($"#{id}");
        await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = timeoutMs });
        await Assertions.Expect(locator)
            .ToHaveTextAsync(text, new LocatorAssertionsToHaveTextOptions { Timeout = timeoutMs });
    }

    internal async Task FillInput(string id, string text) {
        var input = page.Locator($"#{id}");
        await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });
        await input.FillAsync(text);
    }

    internal async Task SetCheckbox(string id, bool value) {
        var checkbox = page.Locator($"#{id}");
        await checkbox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        var isChecked = await checkbox.IsCheckedAsync();
        if (isChecked == value) {
            return;
        }

        if (value) {
            await checkbox.CheckAsync();
        } else {
            await checkbox.UncheckAsync();
        }
    }

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
        var portalConfig = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(solutionRoot, "TradingPortal"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var appDbConnectionString = portalConfig.GetConnectionString("AppDB");
        var testConnectionString = BuildTestConnectionString(appDbConnectionString);

        var startInfo = new ProcessStartInfo {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --urls {baseUrl}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = solutionRoot
        };
        if (!string.IsNullOrWhiteSpace(testConnectionString)) {
            startInfo.Environment["ConnectionStrings__AppDB"] = testConnectionString;
        }

        portalProcess = Process.Start(startInfo);
    }

    private static string? BuildTestConnectionString(string? connectionString) {
        if (string.IsNullOrWhiteSpace(connectionString)) {
            return null;
        }

        var builder = new DbConnectionStringBuilder {
            ConnectionString = connectionString
        };
        var databaseName = $"AppDb_Test_{Guid.NewGuid():N}";
        if (builder.ContainsKey("Database")) {
            builder["Database"] = databaseName;
        } else if (builder.ContainsKey("Initial Catalog")) {
            builder["Initial Catalog"] = databaseName;
        } else {
            builder["Database"] = databaseName;
        }

        return builder.ConnectionString;
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
