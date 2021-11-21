using System;
using System.Collections;
using System.Collections.Generic;
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

        public override bool CanExecute(object? parameter)
        {
            if (parameter is not IList list)
            {
                return false;
            }

            return list.Count > 0;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is not IList<FileViewModel> list)
            {
                return;
            }

            var selectedItems = list.ToList();

            foreach (var selectedItem in selectedItems)
            {
                itemViewModel.Files.Remove(selectedItem);
            }
        }
    }
}
