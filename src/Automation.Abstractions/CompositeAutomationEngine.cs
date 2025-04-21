using Automation.Abstractions;

namespace Automation.Core
{
    public class CompositeAutomationEngine : IAutomationEngine
    {
        private readonly IAutomationFactory _factory;
        private readonly EngineType[] _order;

        private IAutomationEngine? _inner;

        public CompositeAutomationEngine(IAutomationFactory factory, params EngineType[] order)
        {
            _factory = factory;
            _order = order;
        }

        public async Task InitializeAsync()
        {
            foreach (var type in _order)
            {
                try
                {
                    _inner = await _factory.CreateAsync(type);
                    return;
                }
                catch
                {
                    // log and try next
                }
            }
            throw new InvalidOperationException("No engine could be created.");
        }

        public Task NavigateAsync(string url)
            => _inner!.NavigateAsync(url);
        public Task ClickElementAsync(string sel)
            => _inner!.ClickElementAsync(sel);
        public Task EnterTextAsync(string sel, string text)
            => _inner!.EnterTextAsync(sel, text);
        public Task<string> ReadTextAsync(string sel)
            => _inner!.ReadTextAsync(sel);

        public ValueTask DisposeAsync()
            => _inner != null
                ? _inner.DisposeAsync()
                : ValueTask.CompletedTask;
    }
}
