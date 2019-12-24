using System;
using System.Threading.Tasks;

namespace Catalog.Wpf.Commands
{
    public class AsyncDelegateCommand : AsyncCommandBase
    {
        private readonly Func<object?, Task> execute;
        private readonly Predicate<object?>? canExecute;

        public AsyncDelegateCommand(Func<object?, Task> execute, Predicate<object>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        protected override bool CanExecuteImpl(object? parameter) => canExecute?.Invoke(parameter) ?? true;

        protected override Task Perform(object? parameter) => execute.Invoke(parameter);
    }
}
