using Automation.Core;
using Automation.Tasks;
using System;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Automation.Tests;

public class WorkflowLoaderTests
{
    [Fact]
    public void Resolves_Tasks_From_Loaded_Assemblies()
    {
        var json = JsonSerializer.Serialize(new[]
        {
            new { Id = "step", TaskType = typeof(WriteFileTask).FullName, Dependencies = Array.Empty<string>(), Priority = 0 }
        });

        var steps = WorkflowLoader.FromJson(json).ToList();

        Assert.Single(steps);
        Assert.Equal(typeof(WriteFileTask), steps[0].TaskType);
    }
}

