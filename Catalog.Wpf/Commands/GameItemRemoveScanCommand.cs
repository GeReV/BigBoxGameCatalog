using System;
using System.Collections;
using System.Collections.Generic;
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
            if (parameter is not IList<ImageViewModel> list)
            {
                return;
            }

            var selectedItems = list.ToList();

            foreach (var selectedItem in selectedItems)
            {
                itemViewModel.Scans.Remove(selectedItem);
            }
        }
    }
}
