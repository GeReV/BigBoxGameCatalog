using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class AddGameItemCommand : CommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        public AddGameItemCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        public override void Execute(object? parameter)
        {
            var item = new ItemViewModel
            {
                ItemType = ItemTypes.BigBox
            };

            if (parameter is ItemType itemType)
            {
                item.ItemType = itemType;
            }

            editGameViewModel.GameItems.Add(item);
            editGameViewModel.CurrentGameItem = item;
        }
    }
}
