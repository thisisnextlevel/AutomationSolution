using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Automation.Core;
using Automation.Tasks;
using Xunit;

public class DownloadFileTaskTests
{
    private class FailingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new HttpRequestException("network failure");
        }
    }

    [Fact]
    public async Task Sets_error_message_when_download_fails()
    {
        var context = new AutomationContext();
        context.Set("download-url", "http://example.com");
        context.Set("download-path", "dummy.bin");

        var client = new HttpClient(new FailingHandler());
        var task = new DownloadFileTask(client);

        await task.ExecuteAsync(context);

        Assert.Equal("network failure", context.Get<string>("download-error"));
        Assert.Null(context.Get<string>("downloaded-file"));
    }
}

