using System;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class RemoveGameItemCommand : CommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        public RemoveGameItemCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            return parameter is ItemViewModel;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is not ItemViewModel viewModel)
            {
                return;
            }

            editGameViewModel.GameItems.Remove(viewModel);
        }
    }
}
