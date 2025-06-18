using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Automation.Core;

namespace Automation.Tasks
{
    /// <summary>
    /// Downloads a file from a URL provided in the context.
    /// </summary>
    public class DownloadFileTask : IAutomationTask
    {
        private readonly HttpClient _client;

        public DownloadFileTask(HttpClient? client = null)
        {
            _client = client ?? new HttpClient();
        }

        public async Task ExecuteAsync(AutomationContext context)
        {
            var url = context.Get<string>("download-url");
            if (string.IsNullOrWhiteSpace(url))
                return;

            var path = context.Get<string>("download-path") ?? "download.bin";
            try
            {
                var data = await _client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(path, data);
                context.Set("downloaded-file", path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadFileTask error: {ex.Message}");
                context.Set("download-error", ex.Message);
            }
        }
    }
}
