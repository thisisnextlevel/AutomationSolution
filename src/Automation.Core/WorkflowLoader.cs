using System;
using System.Linq;
using System.Text.Json;

namespace Automation.Core
{
    /// <summary>
    /// Parses a JSON workflow definition into workflow steps.
    /// </summary>
    public static class WorkflowLoader
    {
        public static IEnumerable<WorkflowStep> FromJson(string json)
        {
            var items = JsonSerializer.Deserialize<List<WorkflowStepDto>>(json) ?? new();
            foreach (var i in items)
            {
                var type = Type.GetType(i.TaskType)
                    ?? AppDomain.CurrentDomain.GetAssemblies()
                        .Select(a => a.GetType(i.TaskType))
                        .FirstOrDefault(t => t != null)
                    ?? throw new InvalidOperationException($"Task type {i.TaskType} not found");

                yield return new WorkflowStep(i.Id, type, i.Dependencies ?? Array.Empty<string>(), i.Priority);
            }
        }

        private record WorkflowStepDto(string Id, string TaskType, string[]? Dependencies, int Priority);
    }
}
