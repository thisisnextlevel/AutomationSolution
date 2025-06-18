using System;
using System.IO;
using System.Collections.Generic;
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
            services.AddTransient<WriteFileTask>();
            services.AddTransient<RunProcessTask>();
            services.AddTransient<SleepTask>();
            services.AddTransient<DownloadFileTask>();
            services.AddTransient<IAutomationTask, SendChatGPTMessageTask>();
            services.AddTransient<IAutomationTask, WriteFileTask>();
            services.AddTransient<IAutomationTask, RunProcessTask>();
            services.AddTransient<IAutomationTask, SleepTask>();
            services.AddTransient<IAutomationTask, DownloadFileTask>();

            // Load additional tasks or engines from plugins directory
            PluginLoader.LoadPlugins(services, Path.Combine(AppContext.BaseDirectory, "plugins"));

            var sp = services.BuildServiceProvider();

            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Running workflow...");

            var context = sp.GetRequiredService<AutomationContext>();
            context.Set("download-url", "https://example.com");
            context.Set("download-path", "download.bin");
            context.Set("message", "Hello from the refined framework!");
            context.Set("file-path", "output.txt");
            context.Set("file-content", "Automation run at " + DateTime.Now);
            context.Set("command", "echo Workflow complete");
            context.Set("sleep-ms", 1000);

            var workflow = new WorkflowEngine(sp, context);

            IEnumerable<WorkflowStep> steps;
            var wfPath = Path.Combine(AppContext.BaseDirectory, "workflow.json");
            if (File.Exists(wfPath))
            {
                var json = File.ReadAllText(wfPath);
                steps = WorkflowLoader.FromJson(json);
            }
            else
            {
                steps = new[]
                {
                    new WorkflowStep("send", typeof(SendChatGPTMessageTask), Array.Empty<string>())
                };
            }

            await workflow.ExecuteAsync(steps);

            logger.LogInformation("Done.");
        }
    }
}

