using Automation.Core;
using Microsoft.SemanticKernel;

namespace Automation.SemanticKernel
{
    /// <summary>
    /// Runs a Semantic Kernel prompt from the automation context.
    /// </summary>
    public class SemanticKernelTask : IAutomationTask
    {
        public async Task ExecuteAsync(AutomationContext context)
        {
            var prompt = context.Get<string>("sk-prompt");
            if (string.IsNullOrWhiteSpace(prompt))
                return;

            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-3.5-turbo";

            var builder = Kernel.CreateBuilder();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                builder.AddOpenAIChatCompletion(model, apiKey);
            }
            var kernel = builder.Build();

            var result = await kernel.InvokePromptAsync(prompt);
            context.Set("sk-result", result.GetValue<string>() ?? string.Empty);
        }
    }
}
