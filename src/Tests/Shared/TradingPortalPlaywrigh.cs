using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace Tests.Shared;

public class TradingPortalPlaywrigh : IAsyncLifetime {
    private Process? portalProcess;
    private IPlaywright playwright = null!;
    private IBrowser browser = null!;
    private IBrowserContext? browserContext;
    private IPage page = null!;
    private readonly List<IBrowserContext> browserContexts = new();
    private string baseUrl = null!;

    public async Task InitializeAsync() {
        Microsoft.Playwright.Program.Main(["install"]);

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
        await WaitForPortalAsync();
    }

    public async Task GoToPage(string relativeUrl) {
        if (browserContext is not null) {
            await browserContext.CloseAsync();
        }

        browserContext = await browser.NewContextAsync();
        browserContexts.Add(browserContext);
        this.page = await browserContext.NewPageAsync();
        var absoluteUrl = $"{baseUrl!.TrimEnd('/')}/{relativeUrl}";
        await this.page.GotoAsync(absoluteUrl);
    }

    public Task ClickOnButton(string name) {
        var button = page.GetByRole(AriaRole.Button, new() { Name = name });
        return button.ClickAsync(new LocatorClickOptions { Force = true });
    }

    public  Task ShouldBe(string id, string expectedContent, int timeoutMs = 2000) {

         Expect(page.Locator($"#{id}")).ToHaveTextAsync(expectedContent);

    }

    public async Task DisposeAsync() {
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
            Arguments = $"run --project \"{projectPath}\" --urls {baseUrl}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = solutionRoot
        };

        portalProcess = Process.Start(startInfo);
    }

    private async Task WaitForPortalAsync() {
        using var client = new HttpClient();
        var timeout = TimeSpan.FromSeconds(20);
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
