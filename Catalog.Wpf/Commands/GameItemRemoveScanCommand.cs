using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class GameItemRemoveScanCommand : CommandBase
    {
        private readonly ItemViewModel itemViewModel;

        public GameItemRemoveScanCommand(ItemViewModel itemViewModel)
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
            var selectedItems = ((IList) parameter).Cast<ImageViewModel>().ToList();

            foreach (var selectedItem in selectedItems)
            {
                itemViewModel.Scans.Remove(selectedItem);
            }
        }

        public override event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}