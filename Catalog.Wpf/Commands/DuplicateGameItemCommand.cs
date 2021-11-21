using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class DuplicateGameItemCommand : CommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        public DuplicateGameItemCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is not ItemViewModel itemViewModel)
            {
                return;
            }

            var item = itemViewModel.Clone();

            editGameViewModel.GameItems.Add(item);
            editGameViewModel.CurrentGameItem = item;
        }

        public override bool CanExecute(object? parameter) => parameter != null;
    }
}
