using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace Tests.Shared;

public class TradingPortalPlaywrigh : IAsyncLifetime {
    public IPlaywright PlaywrightInstance { get; private set; } = default!;
    public IBrowser Browser { get; private set; } = default!;
    private readonly List<IBrowserContext> _contexts = new();
    private string _baseUrl = default!;
    private Process? _portalProcess;
    private IPage? _page;

    public async Task InitializeAsync() {
        Microsoft.Playwright.Program.Main(new[] { "install" });

        PlaywrightInstance = await Playwright.CreateAsync();
        Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
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
            ? envBaseUrl!
            : !string.IsNullOrWhiteSpace(configBaseUrl)
                ? configBaseUrl!
                : "http://127.0.0.1:5055";

        if (string.IsNullOrWhiteSpace(envBaseUrl) && !await IsPortalReachableAsync(_baseUrl)) {
            StartTradingPortal();
            await WaitForPortalAsync();
        }
    }

    public async Task<(IPage, string)> GetPage() {
        var context = await Browser.NewContextAsync();
        _contexts.Add(context);
        var page = await context.NewPageAsync();
        return (page, _baseUrl);
    }


    public async Task GoToPage(string page) {
        var context = await Browser.NewContextAsync();
        _contexts.Add(context);
        _page = await context.NewPageAsync();
        await _page.GotoAsync($"{_baseUrl}/{page}");
    }

    public Task ClickOnButton(string name ){
        return _page!.GetByRole(AriaRole.Button, new() { Name = name }).ClickAsync();
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
        if (Browser is not null) {
            await Browser.CloseAsync();
        }

        PlaywrightInstance?.Dispose();
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

    private async Task<bool> IsPortalReachableAsync(string baseUrl) {
        using var client = new HttpClient();
        try {
            using var response = await client.GetAsync(baseUrl);
            return response.IsSuccessStatusCode;
        } catch {
            return false;
        }
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
