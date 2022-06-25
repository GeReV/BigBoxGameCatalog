using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Catalog.Wpf.Commands
{
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        private bool isExecuting;

        #region IAsyncCommand Members

        public async Task ExecuteAsync(object? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    isExecuting = true;

                    RaiseCanExecuteChanged();

                    await Task.Run(() => Perform(parameter));
                }
                finally
                {
                    isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public virtual event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        public bool CanExecute(object? parameter)
        {
            return !isExecuting && CanExecuteImpl(parameter);
        }

        protected virtual bool CanExecuteImpl(object? parameter)
        {
            return true;
        }

        protected abstract Task Perform(object? parameter);

        public static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #region Explicit implementations

        bool ICommand.CanExecute(object? parameter)
        {
            return CanExecute(parameter);
        }

#pragma warning disable 4014
        void ICommand.Execute(object? parameter)
        {
            ExecuteAsync(parameter);
        }
#pragma warning restore 4014

        #endregion
    }
}
