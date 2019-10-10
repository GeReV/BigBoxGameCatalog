using System;
using System.Windows.Input;

namespace Catalog.Wpf.Commands
{
    public abstract class CommandBase : ICommand
    {
        public virtual bool CanExecute(object parameter) => true;

        public abstract void Execute(object parameter);

        public virtual event EventHandler CanExecuteChanged;
    }
}