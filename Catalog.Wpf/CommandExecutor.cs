using System.Windows.Input;

namespace Catalog.Wpf
{
    public static class CommandExecutor
    {
        public static bool Execute(ICommand command, object? parameter = null)
        {
            if (!command.CanExecute(parameter))
            {
                return false;
            }

            command.Execute(parameter);

            return true;
        }
    }
}
