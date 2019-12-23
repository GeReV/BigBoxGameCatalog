using System.Threading.Tasks;
using System.Windows.Input;

namespace Catalog.Wpf.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object? parameter);
    }
}
