using Microsoft.Extensions.DependencyInjection;

namespace Automation.Core
{
    /// <summary>
    /// Executes tasks based on dependency order. Tasks whose dependencies are met
    /// are run in parallel, prioritizing higher values first.
    /// </summary>
    public class WorkflowEngine
    {
        private readonly IServiceProvider _provider;
        private readonly AutomationContext _context;

        public WorkflowEngine(IServiceProvider provider, AutomationContext context)
        {
            _provider = provider;
            _context = context;
        }

        public async Task ExecuteAsync(IEnumerable<WorkflowStep> steps)
        {
            var remaining = steps.ToDictionary(s => s.Id);
            var completed = new HashSet<string>();

            while (remaining.Count > 0)
            {
                var ready = remaining.Values
                    .Where(s => s.Dependencies.All(d => completed.Contains(d)))
                    .ToList();

                if (ready.Count == 0)
                    throw new InvalidOperationException("Circular or missing dependency");

                var maxPriority = ready.Max(s => s.Priority);
                var toRun = ready.Where(s => s.Priority == maxPriority).ToList();

                await Task.WhenAll(toRun.Select(async step =>
                {
                    var task = (IAutomationTask)_provider.GetRequiredService(step.TaskType);
                    await task.ExecuteAsync(_context);
                    completed.Add(step.Id);
                    remaining.Remove(step.Id);
                }));
            }
        }
    }

    /// <summary>
    /// Describes a task and its dependencies for a workflow.
    /// </summary>
    public record WorkflowStep(string Id, Type TaskType, string[] Dependencies, int Priority = 0);
}
