using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Automation.Core;
using Automation.Tasks;
using Xunit;

public class HttpPostTaskTests
{
    private class EchoHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content == null ? string.Empty : request.Content.ReadAsStringAsync().Result;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"echo:{body}")
            };
            return Task.FromResult(response);
        }
    }

    [Fact]
    public async Task Posts_body_and_stores_response()
    {
        var context = new AutomationContext();
        context.Set("http-url", "http://example.com");
        context.Set("http-body", "hi");

        var client = new HttpClient(new EchoHandler());
        var task = new HttpPostTask(client);

        await task.ExecuteAsync(context);

        Assert.Equal("echo:hi", context.Get<string>("http-response"));
        Assert.Null(context.Get<string>("http-error"));
    }
}
