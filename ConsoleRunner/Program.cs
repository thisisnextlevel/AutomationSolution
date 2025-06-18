using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Automation.Core;
using Automation.Abstractions;
using Automation.Tasks;
using Automation.Engines.CDP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Automation.ConsoleRunner
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddTransient<CDPAutomationEngine>();
            services.AddSingleton<IAutomationFactory, AutomationFactory>();
            services.AddSingleton<AutomationContext>();

            services.AddTransient<SendChatGPTMessageTask>();
            services.AddTransient<WriteFileTask>();
            services.AddTransient<RunProcessTask>();
            services.AddTransient<SleepTask>();
            services.AddTransient<DownloadFileTask>();
            services.AddTransient<LMStudioTask>();
            services.AddTransient<IAutomationTask, SendChatGPTMessageTask>();
            services.AddTransient<IAutomationTask, WriteFileTask>();
            services.AddTransient<IAutomationTask, RunProcessTask>();
            services.AddTransient<IAutomationTask, SleepTask>();
            services.AddTransient<IAutomationTask, DownloadFileTask>();
            services.AddTransient<IAutomationTask, LMStudioTask>();

            PluginLoader.LoadPlugins(services, Path.Combine(AppContext.BaseDirectory, "plugins"));

            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Running workflow...");

            var context = sp.GetRequiredService<AutomationContext>();
            context.Set("download-url", "https://example.com");
            context.Set("download-path", "download.bin");
            context.Set("message", "Hello from the console runner!");
            context.Set("file-path", "output.txt");
            context.Set("file-content", "Automation run at " + DateTime.Now);
            context.Set("command", "echo Workflow complete");
            context.Set("sleep-ms", 1000);
            context.Set("lmstudio-endpoint", "http://localhost:1234/v1/chat/completions");
            context.Set("prompt", "Hello from LM Studio!");
            context.Set("lmstudio-model", "local-model");

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
                steps = new[] { new WorkflowStep("lmstudio", typeof(LMStudioTask), Array.Empty<string>()) };
            }

            await workflow.ExecuteAsync(steps);
            logger.LogInformation("Done.");
        }
    }
}
