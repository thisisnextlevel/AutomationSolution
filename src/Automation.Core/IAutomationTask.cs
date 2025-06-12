namespace Automation.Core
{
    /// <summary>
    /// Represents a unit of work executed by the automation framework.
    /// </summary>
    public interface IAutomationTask
    {
        /// <summary>
        /// Execute the task using the given context.
        /// </summary>
        Task ExecuteAsync(AutomationContext context);
    }
}
