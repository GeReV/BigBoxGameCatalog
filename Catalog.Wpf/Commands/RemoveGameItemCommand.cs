using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class RemoveGameItemCommand : CommandBase
    {
        private readonly AddGameViewModel addGameViewModel;

        public RemoveGameItemCommand(AddGameViewModel addGameViewModel)
        {
            this.addGameViewModel = addGameViewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is ItemViewModel;
        }

        public override void Execute(object parameter)
        {
            addGameViewModel.GameItems.Remove((ItemViewModel) parameter);
        }
    }
}