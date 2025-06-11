namespace Automation.Core
{
    /// <summary>
    /// Shared context object for tasks to exchange data during a workflow.
    /// </summary>
    public class AutomationContext
    {
        private readonly Dictionary<string, object?> _items = new();

        /// <summary>
        /// Retrieve a value from the context.
        /// </summary>
        public T? Get<T>(string key)
        {
            return _items.TryGetValue(key, out var value) ? (T?)value : default;
        }

        /// <summary>
        /// Store a value in the context.
        /// </summary>
        public void Set<T>(string key, T value)
        {
            _items[key] = value;
        }

        /// <summary>
        /// Exposes all values for debugging or serialization.
        /// </summary>
        public IReadOnlyDictionary<string, object?> Items => _items;
    }
}
