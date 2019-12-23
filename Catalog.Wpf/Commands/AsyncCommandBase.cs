using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Catalog.Wpf.Commands
{
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        private bool isExecuting;

        public bool CanExecute(object? parameter)
        {
            return !isExecuting && CanExecuteImpl(parameter);
        }

        protected virtual bool CanExecuteImpl(object? parameter) => true;

        public async Task ExecuteAsync(object? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    isExecuting = true;
                    await Perform(parameter);
                }
                finally
                {
                    isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        protected abstract Task Perform(object? parameter);

        public virtual event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #region Explicit implementations

        bool ICommand.CanExecute(object parameter) => CanExecute(parameter);

#pragma warning disable 4014
        void ICommand.Execute(object parameter) => ExecuteAsync(parameter);
#pragma warning restore 4014

        #endregion
    }
}
