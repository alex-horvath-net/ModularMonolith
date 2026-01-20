using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Playwright;

namespace Tests.Shared;

public class PlaywrightFixture : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; } = default!;
    public IBrowser Browser { get; private set; } = default!;
    private readonly List<IBrowserContext> _contexts = new();
    private string _baseUrl = default!;
    private Process? _portalProcess;

    public async Task InitializeAsync()
    {
        Microsoft.Playwright.Program.Main(new[] { "install" });

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        _baseUrl = Environment.GetEnvironmentVariable("TRADINGPORTAL_BASE_URL") ?? "http://127.0.0.1:5055";

        if (Environment.GetEnvironmentVariable("TRADINGPORTAL_BASE_URL") is null)
        {
            StartTradingPortal();
            await WaitForPortalAsync();
        }
    }

    public async Task<(IPage, string)> GetPage() {
        var context = await Browser.NewContextAsync();
        _contexts.Add(context);
        var page = await context.NewPageAsync();

        var baseUrl = Environment.GetEnvironmentVariable("TRADINGPORTAL_BASE_URL") ?? _baseUrl;
        return (page, baseUrl);
    }

    public async Task DisposeAsync()
    {
        foreach (var context in _contexts)
        {
            await context.CloseAsync();
        }

        if (_portalProcess is not null && !_portalProcess.HasExited)
        {
            try
            {
                _portalProcess.Kill(entireProcessTree: true);
            }
            catch
            {
                // ignore best-effort shutdown
            }
        }
        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
    }

    private void StartTradingPortal()
    {
        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var projectPath = Path.Combine(solutionRoot, "TradingPortal", "TradingPortal.csproj");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --urls {_baseUrl}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = solutionRoot
        };

        _portalProcess = Process.Start(startInfo);
    }

    private async Task WaitForPortalAsync()
    {
        using var client = new HttpClient();
        var timeout = TimeSpan.FromSeconds(20);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            try
            {
                using var response = await client.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch
            {
                // retry until timeout
            }

            await Task.Delay(500);
        }

        throw new InvalidOperationException($"TradingPortal did not start at {_baseUrl} within {timeout.TotalSeconds} seconds.");
    }
}
