using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Automation.Core;
using Automation.Tasks;
using Xunit;

public class HttpGetTaskTests
{
    private class SuccessHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("hello")
            };
            return Task.FromResult(response);
        }
    }

    [Fact]
    public async Task Stores_response_text_in_context()
    {
        var context = new AutomationContext();
        context.Set("http-url", "http://example.com");

        var client = new HttpClient(new SuccessHandler());
        var task = new HttpGetTask(client);

        await task.ExecuteAsync(context);

        Assert.Equal("hello", context.Get<string>("http-response"));
        Assert.Null(context.Get<string>("http-error"));
    }
}
