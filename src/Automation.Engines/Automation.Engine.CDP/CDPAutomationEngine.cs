using Automation.Abstractions;
using PuppeteerSharp;
using System.Diagnostics;
using System.Linq;

namespace Automation.Engines.CDP
{
    public class CDPAutomationEngine : IAutomationEngine
    {
        private IBrowser? _browser;
        private IPage? _page;
        private readonly int _debugPort = 9222;
        private readonly string _profilePath;
        private string? _chromePath;

        public CDPAutomationEngine()
        {
            _chromePath = ChromeFinder.FindChromeExecutable();
            _profilePath = Environment.GetEnvironmentVariable("CHROME_PROFILE") ?? "Profile 1";
            if (int.TryParse(Environment.GetEnvironmentVariable("CHROME_DEBUG_PORT"), out var port))
                _debugPort = port;
        }

        public async Task LaunchChromeWithDebuggingAsync()
        {
            var exe = _chromePath;
            if (exe is null)
            {
                var fetcher = new BrowserFetcher();
                await fetcher.DownloadAsync(Chrome.DefaultBuildId);
                exe = fetcher.GetExecutablePath(Chrome.DefaultBuildId);
                _chromePath = exe;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = exe,
                Arguments = $"--remote-debugging-port={_debugPort} --profile-directory=\"{_profilePath}\"",
                UseShellExecute = true
            });
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Try attach
                _browser = await Puppeteer.ConnectAsync(new ConnectOptions
                {
                    BrowserURL = $"http://localhost:{_debugPort}"
                });
            }
            catch
            {
                // Launch a fresh instance if no debugging endpoint found
                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = false,
                    ExecutablePath = _chromePath,
                    Args = new[]
                    {
                        "--start-maximized",         // open the window maximized
                        $"--remote-debugging-port={_debugPort}",
                        $"--user-data-dir={_profilePath}",
     //   $"--profile-directory={ProfileDirectory}"
                    }
                });
            }

            var pages = await _browser.PagesAsync();
            _page = pages.FirstOrDefault() ?? await _browser.NewPageAsync();
        }

        public Task NavigateAsync(string url)
            => _page.GoToAsync(url);

        public Task ClickElementAsync(string selector)
            => _page.ClickAsync(selector);

        public async Task EnterTextAsync(string selector, string text)
        {
            await _page.FocusAsync(selector);
            await _page.Keyboard.TypeAsync(text);
        }

        public Task<string> ReadTextAsync(string selector)
            => _page.EvaluateExpressionAsync<string>($"document.querySelector('{selector}')?.innerText || ''");

        public async ValueTask DisposeAsync()
        {
            if (_browser != null)
                await _browser.CloseAsync();
        }
    }
}
