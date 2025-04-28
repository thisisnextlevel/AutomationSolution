using System;
using System.Threading.Tasks;
using Automation.Core;
using Automation.Tasks;
using Automation.Engines.CDP;
using Automation.Engines.UIAutomation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Automation.Runner
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // 1) Build the DI container
            var services = new ServiceCollection();

            // Logging—this makes AddConsole() available
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Register the two engines
            services.AddTransient<CDPAutomationEngine>();
            services.AddTransient<UIAutomationEngine>();

            // Register the factory (in Runner) that knows about both engines
            services.AddSingleton<IAutomationFactory, AutomationFactory>();

            // Register your tasks
            services.AddTransient<SendChatGPTMessageTask>();

            var sp = services.BuildServiceProvider();

            // 2) Resolve a logger and your task, then run
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting ChatGPT message task…");

            var task = sp.GetRequiredService<SendChatGPTMessageTask>();
            await task.ExecuteAsync("Hello from the refined framework!");

            logger.LogInformation("Done.");
        }
    }
}
