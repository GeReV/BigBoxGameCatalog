using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Catalog.Wpf.ViewModel;
using Microsoft.Win32;

namespace Catalog.Wpf.Commands
{
    public class GameItemRemoveFileCommand : CommandBase
    {
        private readonly ItemViewModel itemViewModel;

        public GameItemRemoveFileCommand(ItemViewModel itemViewModel)
        {
            this.itemViewModel = itemViewModel;
        }

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is IList list))
            {
                return false;
            }

            return list.Count > 0;
        }

        public override void Execute(object parameter)
        {
            var selectedItems = ((IList)parameter).Cast<FileViewModel>().ToList();

            foreach (var selectedItem in selectedItems)
            {
                itemViewModel.Files.Remove(selectedItem);
            }
        }

        public override event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}