using Automation.Abstractions;
using Automation.Core;
using Automation.Engines.CDP;
#if WINDOWS
using Automation.Engines.UIAutomation;
#endif
using Automation.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Automation.Runtime;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutomation(this IServiceCollection services, bool includeUIAutomation = false)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddTransient<CDPAutomationEngine>();
#if WINDOWS
        if (includeUIAutomation)
            services.AddTransient<UIAutomationEngine>();
#endif
        services.AddSingleton<AutomationContext>();

        services.AddTransient<SendChatGPTMessageTask>();
        services.AddTransient<WriteFileTask>();
        services.AddTransient<RunProcessTask>();
        services.AddTransient<SleepTask>();
        services.AddTransient<DownloadFileTask>();
        services.AddTransient<HttpGetTask>();
        services.AddTransient<HttpPostTask>();
        services.AddTransient<LMStudioTask>();

        services.AddTransient<IAutomationTask, SendChatGPTMessageTask>();
        services.AddTransient<IAutomationTask, WriteFileTask>();
        services.AddTransient<IAutomationTask, RunProcessTask>();
        services.AddTransient<IAutomationTask, SleepTask>();
        services.AddTransient<IAutomationTask, DownloadFileTask>();
        services.AddTransient<IAutomationTask, HttpGetTask>();
        services.AddTransient<IAutomationTask, HttpPostTask>();
        services.AddTransient<IAutomationTask, LMStudioTask>();

        PluginLoader.LoadPlugins(services, Path.Combine(AppContext.BaseDirectory, "plugins"));

#if WINDOWS
        services.AddSingleton<IAutomationFactory>(sp => new AutomationFactory(sp, includeUIAutomation));
#else
        services.AddSingleton<IAutomationFactory, AutomationFactory>();
#endif
        return services;
    }
}
