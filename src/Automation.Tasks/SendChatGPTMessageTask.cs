using System;
using System.Threading.Tasks;
using Automation.Abstractions;
using Automation.Core;

namespace Automation.Tasks
{
    /// <summary>
    /// A task that sends a message to ChatGPT via the automation engine.
    /// </summary>
    public class SendChatGPTMessageTask : IAutomationTask
    {
        private readonly IAutomationFactory _factory;
        private readonly AutomationContext _context;

        public SendChatGPTMessageTask(IAutomationFactory factory, AutomationContext context)
        {
            _factory = factory;
            _context = context;
        }

        /// <summary>
        /// Executes the task: navigates to ChatGPT, enters a message, and sends it.
        /// </summary>
        public async Task ExecuteAsync(AutomationContext context)
        {
            var message = context.Get<string>("message") ?? string.Empty;

            await using var engine = await _factory.CreateAsync(EngineType.CDP);

            await engine.NavigateAsync("https://chat.openai.com/");

            await Task.Delay(2000);

            await engine.EnterTextAsync("textarea", message);
            await engine.ClickElementAsync("button[type='submit']");

            context.Set("last-message", message);
        }
    }
}
