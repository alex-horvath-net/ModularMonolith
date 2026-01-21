using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace Tests.Shared;

public class TradingPortalPlaywrigh : IAsyncLifetime {
    public IPlaywright _playwrigt { get; private set; } = default!;
    public IBrowser _browser { get; private set; } = default!;
    private readonly List<IBrowserContext> _contexts = new();
    private string?_baseUrl = default!;
    private IBrowserContext? _currentContext;
    private Process? _portalProcess;
    private IPage? _page;

    public async Task InitializeAsync() {
        Microsoft.Playwright.Program.Main(new[] { "install" });

        _playwrigt = await Playwright.CreateAsync();
        _browser = await _playwrigt.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
            Headless = true
        });

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();


        var envBaseUrl = Environment.GetEnvironmentVariable("TRADINGPORTAL_BASE_URL");
        var configBaseUrl = configuration["Playwright:BaseUrl"];

        _baseUrl = !string.IsNullOrWhiteSpace(envBaseUrl)
            ? envBaseUrl
            : !string.IsNullOrWhiteSpace(configBaseUrl)
                ? configBaseUrl
                : "http://127.0.0.1:5055";

        StartTradingPortal();
        await WaitForPortalAsync();
    }

    public async Task GoToPage(string page) {
        if (_currentContext is not null) {
            await _currentContext.CloseAsync();
        }

        _currentContext = await _browser.NewContextAsync();
        _contexts.Add(_currentContext);
        _page = await _currentContext.NewPageAsync();
        await _page.GotoAsync($"{_baseUrl!.TrimEnd('/')}/{page}");
    }

    public Task ClickOnButton(string name) {
        if (_page is null) {
            throw new InvalidOperationException("No active page. Call GoToPage first.");
        }

        return _page.GetByRole(AriaRole.Button, new() { Name = name }).ClickAsync();
    }

    public async Task WaitForWorkflowStartedAsync(string testId, int timeoutMs = 2000) {
        if (_page is null) {
            throw new InvalidOperationException("No active page. Call GoToPage first.");
        }

        await _page.GetByTestId(testId).WaitForAsync(new LocatorWaitForOptions {
            Timeout = timeoutMs,
            State = WaitForSelectorState.Visible
        });
    }

    public async Task WaitForTextAsync(string text, int timeoutMs = 2000) {
        if (_page is null) {
            throw new InvalidOperationException("No active page. Call GoToPage first.");
        }

        await _page.GetByText(text, new() { Exact = true }).WaitForAsync(new LocatorWaitForOptions {
            Timeout = timeoutMs,
            State = WaitForSelectorState.Visible
        });
    }

    public async Task DisposeAsync() {
        foreach (var context in _contexts) {
            await context.CloseAsync();
        }

        if (_portalProcess is not null && !_portalProcess.HasExited) {
            try {
                _portalProcess.Kill(entireProcessTree: true);
            } catch {
                // ignore best-effort shutdown
            }
        }
        if (_browser is not null) {
            await _browser.CloseAsync();
        }

        _playwrigt?.Dispose();
    }

    private void StartTradingPortal() {
        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var projectPath = Path.Combine(solutionRoot, "TradingPortal", "TradingPortal.csproj");

        var startInfo = new ProcessStartInfo {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --urls {_baseUrl}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = solutionRoot
        };

        _portalProcess = Process.Start(startInfo);
    }


    private async Task WaitForPortalAsync() {
        using var client = new HttpClient();
        var timeout = TimeSpan.FromSeconds(20);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout) {
            try {
                using var response = await client.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode) {
                    return;
                }
            } catch {
                // retry until timeout
            }

            await Task.Delay(500);
        }

        throw new InvalidOperationException($"TradingPortal did not start at {_baseUrl} within {timeout.TotalSeconds} seconds.");
    }
}
