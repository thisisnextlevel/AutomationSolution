namespace Automation.Abstractions
{
    public interface IAutomationEngine : IAsyncDisposable
    {
        /// <summary>
        /// Prepare the engine (launch or attach to browser, find root window, etc).
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Navigate (or activate) to the given URL.
        /// </summary>
        Task NavigateAsync(string url);

        /// <summary>
        /// Click the element matched by CSS selector (or XPath, etc).
        /// </summary>
        Task ClickElementAsync(string selector);

        /// <summary>
        /// Enter text into the matched input.
        /// </summary>
        Task EnterTextAsync(string selector, string text);

        /// <summary>
        /// Read innerText (or value) from the matched element.
        /// </summary>
        Task<string> ReadTextAsync(string selector);
    }
    public enum EngineType
    {
        CDP,
        UIAutomation
    }
}

