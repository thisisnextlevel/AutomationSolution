
using Automation.Engines.CDP;
using Automation.Engines.UIAutomation;
using Automation.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLogging(cfg => cfg.AddConsole().SetMinimumLevel(LogLevel.Debug));

// register engines + factory + tasks
services.AddTransient<CDPAutomationEngine>();
services.AddTransient<UIAutomationEngine>();
services.AddSingleton<AutomationFactory>();
services.AddTransient<SendChatGPTMessageTask>();

var sp = services.BuildServiceProvider();
var logger = sp.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting automation runner...");

var task = sp.GetRequiredService<SendChatGPTMessageTask>();
await task.ExecuteAsync("Hello from .NET Automation Framework!");

logger.LogInformation("Done!");
