using System;
using System.IO;
using System.Runtime.InteropServices;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;

namespace Automation.Engines.CDP
{
    internal static class ChromeFinder
    {
        public static string? FindChromeExecutable()
        {
            foreach (var name in new[] { "CHROME_EXECUTABLE", "CHROME_PATH", "CHROME_BIN", "CHROME", "GOOGLE_CHROME_SHIM", "EDGE_EXECUTABLE" })
            {
                var env = Environment.GetEnvironmentVariable(name);
                if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
                    return env;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return FindChromeOnWindows() ?? FindEdgeOnWindows() ?? FindPuppeteerChromium();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return FindChromeOnMac() ?? FindEdgeOnMac() ?? FindPuppeteerChromium();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return FindChromeOnLinux() ?? FindEdgeOnLinux() ?? FindPuppeteerChromium();

            return FindPuppeteerChromium();
        }

        private static string? FindChromeOnWindows()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var path = Path.Combine(programFiles, "Google", "Chrome", "Application", "chrome.exe");
            if (File.Exists(path))
                return path;

            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var pathX86 = Path.Combine(programFilesX86, "Google", "Chrome", "Application", "chrome.exe");
            if (File.Exists(pathX86))
                return pathX86;

            return null;
        }

        private static string? FindChromeOnMac()
        {
            const string macPath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
            return File.Exists(macPath) ? macPath : null;
        }

        private static string? FindChromeOnLinux()
        {
            string[] linuxPaths =
            {
                "/usr/bin/google-chrome",
                "/usr/bin/google-chrome-stable",
                "/usr/bin/chromium-browser",
                "/usr/bin/chromium",
            };
            foreach (var p in linuxPaths)
                if (File.Exists(p))
                    return p;
            return null;
        }

        private static string? FindEdgeOnWindows()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var path = Path.Combine(programFiles, "Microsoft", "Edge", "Application", "msedge.exe");
            if (File.Exists(path))
                return path;

            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var pathX86 = Path.Combine(programFilesX86, "Microsoft", "Edge", "Application", "msedge.exe");
            return File.Exists(pathX86) ? pathX86 : null;
        }

        private static string? FindEdgeOnMac()
        {
            const string macPath = "/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge";
            return File.Exists(macPath) ? macPath : null;
        }

        private static string? FindEdgeOnLinux()
        {
            string[] linuxPaths =
            {
                "/usr/bin/microsoft-edge",
                "/usr/bin/microsoft-edge-stable",
            };
            foreach (var p in linuxPaths)
                if (File.Exists(p))
                    return p;
            return null;
        }

        private static string? FindPuppeteerChromium()
        {
            var fetcher = new BrowserFetcher();
            var path = fetcher.GetExecutablePath(Chrome.DefaultBuildId);
            return File.Exists(path) ? path : null;
        }
    }
}
