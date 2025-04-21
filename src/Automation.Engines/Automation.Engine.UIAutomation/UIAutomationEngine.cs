using Automation.Abstractions;
using System;
using System.Linq;
using System.Windows.Automation;

namespace Automation.Engines.UIAutomation
{
    public class UIAutomationEngine : IAutomationEngine
    {
        private AutomationElement? _root;

        public Task InitializeAsync()
        {
            _root = AutomationElement.RootElement;
            if (_root == null)
                throw new InvalidOperationException("UI Automation RootElement not available.");
            return Task.CompletedTask;
        }

        public Task NavigateAsync(string url)
        {
            // no-op for UIA; rely on CDP for browser navigation
            return Task.CompletedTask;
        }

        public Task ClickElementAsync(string selector)
        {
            // Example: treat selector as NameProperty
            var el = _root.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, selector));
            if (el != null)
            {
                var invoke = el.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                invoke?.Invoke();
            }
            return Task.CompletedTask;
        }

        public Task EnterTextAsync(string selector, string text)
        {
            var el = _root.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, selector));
            if (el != null)
            {
                var value = el.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                value?.SetValue(text);
            }
            return Task.CompletedTask;
        }

        public Task<string> ReadTextAsync(string selector)
        {
            var el = _root.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, selector));
            if (el != null)
            {
                var value = el.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                return Task.FromResult(value?.Current.Value ?? string.Empty);
            }
            return Task.FromResult(string.Empty);
        }

        public ValueTask DisposeAsync()
        {
            // nothing to tear down
            return ValueTask.CompletedTask;
        }
    }
}
