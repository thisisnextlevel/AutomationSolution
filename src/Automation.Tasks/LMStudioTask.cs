using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Automation.Core;

namespace Automation.Tasks
{
    /// <summary>
    /// Sends a prompt to an LM Studio API endpoint and stores the response.
    /// </summary>
    public class LMStudioTask : IAutomationTask
    {
        private static readonly HttpClient _client = new();

        public async Task ExecuteAsync(AutomationContext context)
        {
            var endpoint = context.Get<string>("lmstudio-endpoint") ?? "http://localhost:1234/v1/chat/completions";
            var prompt = context.Get<string>("prompt");
            if (string.IsNullOrWhiteSpace(prompt))
                return;

            var model = context.Get<string>("lmstudio-model") ?? "local-model";

            var request = new
            {
                model,
                messages = new[] { new { role = "user", content = prompt } }
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, content);
            var json = await response.Content.ReadAsStringAsync();
            string result;
            try
            {
                using var doc = JsonDocument.Parse(json);
                result = doc.RootElement.GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? json;
            }
            catch
            {
                result = json;
            }

            context.Set("lmstudio-response", result);
        }
    }
}
