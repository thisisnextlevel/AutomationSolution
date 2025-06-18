using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Automation.Core;

namespace Automation.Tasks;

/// <summary>
/// Performs an HTTP POST request with a string body and stores the response text.
/// </summary>
public class HttpPostTask : IAutomationTask
{
    private readonly HttpClient _client;

    public HttpPostTask(HttpClient? client = null)
    {
        _client = client ?? new HttpClient();
    }

    public async Task ExecuteAsync(AutomationContext context)
    {
        var url = context.Get<string>("http-url");
        var body = context.Get<string>("http-body") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(url))
            return;

        try
        {
            using var content = new StringContent(body, Encoding.UTF8, "text/plain");
            var response = await _client.PostAsync(url, content);
            var text = await response.Content.ReadAsStringAsync();
            context.Set("http-response", text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HttpPostTask error: {ex.Message}");
            context.Set("http-error", ex.Message);
        }
    }
}
