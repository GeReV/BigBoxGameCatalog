using System.Threading.Tasks;
using System.Windows.Input;
using Catalog.Wpf.Commands;

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

        public static async Task<bool> Execute(IAsyncCommand command, object? parameter = null)
        {
            if (!command.CanExecute(parameter))
            {
                return false;
            }

            await command.ExecuteAsync(parameter);

            return true;
        }
    }
}
