using System;
using System.Net.Http;
using System.Threading.Tasks;
using Automation.Core;

namespace Automation.Tasks;

/// <summary>
/// Performs an HTTP GET request and stores the response text in the context.
/// </summary>
public class HttpGetTask : IAutomationTask
{
    private readonly HttpClient _client;

    public HttpGetTask(HttpClient? client = null)
    {
        _client = client ?? new HttpClient();
    }

    public async Task ExecuteAsync(AutomationContext context)
    {
        var url = context.Get<string>("http-url");
        if (string.IsNullOrWhiteSpace(url))
            return;

        try
        {
            var text = await _client.GetStringAsync(url);
            context.Set("http-response", text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HttpGetTask error: {ex.Message}");
            context.Set("http-error", ex.Message);
        }
    }
}
