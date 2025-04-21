
using System;
using System.Threading.Tasks;
using Automation.Engines.CDP;
using Automation.Engines.UIAutomation;
using Automation.Core;
using Automation.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Automation.Runner
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // 1. Configure DI
            var services = new ServiceCollection();

            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Register engines (Runner targets net8.0-windows)
            services.AddTransient<CDPAutomationEngine>();
            services.AddTransient<UIAutomationEngine>();

            // Register factory and tasks
            services.AddSingleton<IAutomationFactory, AutomationFactory>();
            services.AddTransient<SendChatGPTMessageTask>();

            var serviceProvider = services.BuildServiceProvider();

            // 2. Run the task
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting SendChatGPTMessageTask...");

            var task = serviceProvider.GetRequiredService<SendChatGPTMessageTask>();
            await task.ExecuteAsync("Hello from .NET Automation!");

            logger.LogInformation("Task completed.");
        }
    }
}
