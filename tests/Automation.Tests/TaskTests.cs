using Automation.Core;
using Automation.Tasks;
using System.Diagnostics;
using Xunit;

namespace Automation.Tests;

public class TaskTests
{
    [Fact]
    public async Task WriteFileTask_Writes_Content()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var ctx = new AutomationContext();
            ctx.Set("file-path", tempFile);
            ctx.Set("file-content", "hello world");

            var task = new WriteFileTask();
            await task.ExecuteAsync(ctx);

            Assert.Equal("hello world", File.ReadAllText(tempFile));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task RunProcessTask_Invokes_Command_Using_Cmd()
    {
        var tempDir = Directory.CreateTempSubdirectory();
        var log = Path.Combine(tempDir.FullName, "log.txt");
        var script = Path.Combine(tempDir.FullName, "cmd");
        await File.WriteAllTextAsync(script, "#!/bin/sh\necho \"$@\" > \"" + log + "\"\n");
        Process.Start("chmod", "+x " + script).WaitForExit();

        var originalPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("PATH", tempDir.FullName + Path.PathSeparator + originalPath);
        try
        {
            var ctx = new AutomationContext();
            ctx.Set("command", "echo hi");
            var task = new RunProcessTask();
            await task.ExecuteAsync(ctx);
            await Task.Delay(100);
            Assert.Equal("/c echo hi", File.ReadAllText(log).Trim());
        }
        finally
        {
            Environment.SetEnvironmentVariable("PATH", originalPath);
            tempDir.Delete(true);
        }
    }

    [Fact]
    public async Task SleepTask_Waits_For_Specified_Time()
    {
        var ctx = new AutomationContext();
        ctx.Set("sleep-ms", 50);
        var sw = Stopwatch.StartNew();
        var task = new SleepTask();
        await task.ExecuteAsync(ctx);
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds >= 40);
    }
}
