using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Catalog.Wpf.Commands
{
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool isExecuting;

        public bool CanExecute(object parameter)
        {
            return !isExecuting && CanExecuteImpl(parameter);
        }

        protected virtual bool CanExecuteImpl(object parameter) => true;

        public async Task ExecuteAsync(object parameter)
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

        protected abstract Task Perform(object parameter);

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter) => CanExecute(parameter);

#pragma warning disable 4014
        void ICommand.Execute(object parameter) => ExecuteAsync(parameter);
#pragma warning restore 4014
        #endregion
    }

}