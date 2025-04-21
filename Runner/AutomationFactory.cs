using Automation.Abstractions;
using Automation.Core;
using Automation.Engines.CDP;
using Automation.Engines.UIAutomation;
using Microsoft.Extensions.DependencyInjection;

namespace Automation.Runner
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
                EngineType.UIAutomation => _sp.GetRequiredService<UIAutomationEngine>(),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (engine is CDPAutomationEngine cdp)
                cdp.LaunchChromeWithDebugging();

            await engine.InitializeAsync();
            return engine;
        }
    }
}
