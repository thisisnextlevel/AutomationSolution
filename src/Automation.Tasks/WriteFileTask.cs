using System.IO;
using System.Threading.Tasks;
using Automation.Core;

namespace Automation.Tasks
{
    /// <summary>
    /// Writes text from the context into a file.
    /// </summary>
    public class WriteFileTask : IAutomationTask
    {
        public Task ExecuteAsync(AutomationContext context)
        {
            var path = context.Get<string>("file-path") ?? "output.txt";
            var message = context.Get<string>("file-content") ?? string.Empty;
            File.WriteAllText(path, message);
            return Task.CompletedTask;
        }
    }
}
