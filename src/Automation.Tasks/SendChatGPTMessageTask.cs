using System;
using System.Threading.Tasks;
using Automation.Abstractions;
using Automation.Core;

namespace Automation.Tasks
{
    /// <summary>
    /// A task that sends a message to ChatGPT via the automation engine.
    /// </summary>
    public class SendChatGPTMessageTask
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
        public async Task ExecuteAsync(string message)
        {
            // Create and initialize the CDP engine
            await using var engine = await _factory.CreateAsync(EngineType.CDP);

            // Navigate to ChatGPT
            await engine.NavigateAsync("https://chat.openai.com/");

            // Wait a moment for page load (or implement a proper wait)
            await Task.Delay(2000);

            // Enter the message and click send
            await engine.EnterTextAsync("textarea", message);
            await engine.ClickElementAsync("button[type='submit']");

            _context.Set("last-message", message);
        }
    }
}