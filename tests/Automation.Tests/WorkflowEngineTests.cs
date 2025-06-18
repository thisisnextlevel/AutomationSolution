using Automation.Core;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Automation.Tests;

public class WorkflowEngineTests
{
    private abstract class RecordingTask : IAutomationTask
    {
        private readonly string _name;
        private readonly List<string> _calls;

        protected RecordingTask(string name, List<string> calls)
        {
            _name = name;
            _calls = calls;
        }

        public Task ExecuteAsync(AutomationContext context)
        {
            _calls.Add(_name);
            return Task.CompletedTask;
        }
    }

    private class TaskA : RecordingTask { public TaskA(List<string> c) : base("A", c) { } }
    private class TaskB : RecordingTask { public TaskB(List<string> c) : base("B", c) { } }
    private class TaskC : RecordingTask { public TaskC(List<string> c) : base("C", c) { } }

    [Fact]
    public async Task Executes_In_Dependency_And_Priority_Order()
    {
        var calls = new List<string>();
        var services = new ServiceCollection();
        services.AddTransient<TaskA>(_ => new TaskA(calls));
        services.AddTransient<TaskB>(_ => new TaskB(calls));
        services.AddTransient<TaskC>(_ => new TaskC(calls));
        var provider = services.BuildServiceProvider();
        var context = new AutomationContext();
        var engine = new WorkflowEngine(provider, context);

        var steps = new[]
        {
            new WorkflowStep("A", typeof(TaskA), Array.Empty<string>(), 0),
            new WorkflowStep("B", typeof(TaskB), new[] { "A" }, 0),
            new WorkflowStep("C", typeof(TaskC), new[] { "A" }, 1)
        };

        await engine.ExecuteAsync(steps);

        Assert.Equal(new[] { "A", "C", "B" }, calls);
    }
}
