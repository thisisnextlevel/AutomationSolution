using System;
using System.IO;
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

            // Shared context for tasks
            services.AddSingleton<AutomationContext>();

            // Register your tasks
            services.AddTransient<SendChatGPTMessageTask>();
            services.AddTransient<IAutomationTask, SendChatGPTMessageTask>();

            // Load additional tasks or engines from plugins directory
            PluginLoader.LoadPlugins(services, Path.Combine(AppContext.BaseDirectory, "plugins"));

            var sp = services.BuildServiceProvider();

            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Running workflow...");

            var context = sp.GetRequiredService<AutomationContext>();
            context.Set("message", "Hello from the refined framework!");

            var workflow = new WorkflowEngine(sp, context);
            await workflow.ExecuteAsync(new[]
            {
                new WorkflowStep("send", typeof(SendChatGPTMessageTask), Array.Empty<string>())
            });

            logger.LogInformation("Done.");
        }
    }
}

