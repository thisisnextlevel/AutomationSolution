using Automation.Abstractions;
using Automation.Core;

namespace Automation.Core
{
    /// <summary>
    /// Factory that can create and initialize IAutomationEngine instances by type.
    /// </summary>
    public interface IAutomationFactory
    {
        /// <param name="type">Which engine to create.</param>
        Task<IAutomationEngine> CreateAsync(EngineType type);
    }
}
