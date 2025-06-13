using System.Diagnostics;
using System.Threading.Tasks;
using Automation.Core;

namespace Automation.Tasks
{
    /// <summary>
    /// Executes a shell command provided in the context.
    /// </summary>
    public class RunProcessTask : IAutomationTask
    {
        public Task ExecuteAsync(AutomationContext context)
        {
            var command = context.Get<string>("command");
            if (string.IsNullOrWhiteSpace(command))
                return Task.CompletedTask;

            var psi = new ProcessStartInfo("cmd", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
            return Task.CompletedTask;
        }
    }
}
