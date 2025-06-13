using System.Threading.Tasks;
using Automation.Core;

namespace Automation.Tasks
{
    /// <summary>
    /// Pauses execution for a number of milliseconds specified in the context.
    /// </summary>
    public class SleepTask : IAutomationTask
    {
        public Task ExecuteAsync(AutomationContext context)
        {
            var ms = context.Get<int?>("sleep-ms") ?? 0;
            return Task.Delay(ms);
        }
    }
}
