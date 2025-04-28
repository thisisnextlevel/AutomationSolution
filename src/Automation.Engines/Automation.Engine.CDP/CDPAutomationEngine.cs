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
        private const int DebugPort = 9222;
        private const string ProfilePath = "Profile 1";

        public void LaunchChromeWithDebugging()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "chrome.exe",
                Arguments = $"--remote-debugging-port={DebugPort} --profile-directory=\"{ProfilePath}\"", //--user-data-dir=\"{ProfilePath}\" 
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
                    BrowserURL = $"http://localhost:{DebugPort}"
                });
            }
            catch
            {
                // Launch a fresh instance if no debugging endpoint found
                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = false,
                    Args = new[]
    {
        "--start-maximized",         // open the window maximized
        "--remote-debugging-port=9222",
        $"--user-data-dir={ProfilePath}",
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
