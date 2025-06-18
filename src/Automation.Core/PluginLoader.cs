using System;
using System.IO;
using System.Reflection;
using Automation.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Automation.Core
{
    /// <summary>
    /// Scans a directory for assemblies and registers automation engines and tasks
    /// with the dependency injection container.
    /// </summary>
    public static class PluginLoader
    {
        public static void LoadPlugins(IServiceCollection services, string pluginDirectory)
        {
            // register built-in integrations shipped with the framework
            var skPath = Path.Combine(AppContext.BaseDirectory, "Automation.SemanticKernel.dll");
            if (File.Exists(skPath))
            {
                var assembly = Assembly.LoadFrom(skPath);
                RegisterTypesFromAssembly(services, assembly);
            }

            if (!Directory.Exists(pluginDirectory))
                return;

            foreach (var dll in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dll);
                RegisterTypesFromAssembly(services, assembly);
            }
        }

        private static void RegisterTypesFromAssembly(IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (typeof(IAutomationEngine).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    services.AddTransient(typeof(IAutomationEngine), type);
                }
                else if (type.Name.EndsWith("Task") && !type.IsAbstract && !type.IsInterface)
                {
                    services.AddTransient(type);
                }
            }
        }
    }
}
