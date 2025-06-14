using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Automation.Engines.CDP
{
    internal static class ChromeFinder
    {
        public static string? FindChromeExecutable()
        {
            var env = Environment.GetEnvironmentVariable("CHROME_EXECUTABLE");
            if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
                return env;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return FindChromeOnWindows();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return FindChromeOnMac();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return FindChromeOnLinux();

            return null;
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
    }
}
