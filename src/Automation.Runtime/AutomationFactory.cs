using Automation.Abstractions;
using Automation.Engines.CDP;
#if WINDOWS
using Automation.Engines.UIAutomation;
#endif
using Microsoft.Extensions.DependencyInjection;

namespace Automation.Runtime;

public class AutomationFactory : IAutomationFactory
{
    private readonly IServiceProvider _sp;
#if WINDOWS
    private readonly bool _useUIA;
    public AutomationFactory(IServiceProvider sp, bool useUIAutomation)
    {
        _sp = sp;
        _useUIA = useUIAutomation;
    }
#else
    public AutomationFactory(IServiceProvider sp)
    {
        _sp = sp;
    }
#endif

    public async Task<IAutomationEngine> CreateAsync(EngineType type)
    {
        IAutomationEngine engine = type switch
        {
            EngineType.CDP => _sp.GetRequiredService<CDPAutomationEngine>(),
#if WINDOWS
            EngineType.UIAutomation when _useUIA => _sp.GetRequiredService<UIAutomationEngine>(),
            EngineType.UIAutomation => throw new PlatformNotSupportedException("UIAutomation engine not available."),
#endif
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        if (engine is CDPAutomationEngine cdp)
            await cdp.LaunchChromeWithDebuggingAsync();

        await engine.InitializeAsync();
        return engine;
    }
}
