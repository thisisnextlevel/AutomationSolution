using Automation.Abstractions;
using Automation.Core;
using Automation.Engines.CDP;
using Microsoft.Extensions.DependencyInjection;

namespace Automation.ConsoleRunner
{
    public class AutomationFactory : IAutomationFactory
    {
        private readonly IServiceProvider _sp;
        public AutomationFactory(IServiceProvider sp) => _sp = sp;

        public async Task<IAutomationEngine> CreateAsync(EngineType type)
        {
            IAutomationEngine engine = type switch
            {
                EngineType.CDP => _sp.GetRequiredService<CDPAutomationEngine>(),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (engine is CDPAutomationEngine cdp)
                await cdp.LaunchChromeWithDebuggingAsync();

            await engine.InitializeAsync();
            return engine;
        }
    }
}
