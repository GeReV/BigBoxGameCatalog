using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class AddGameItemCommand : CommandBase
    {
        private readonly AddGameViewModel addGameViewModel;

        public AddGameItemCommand(AddGameViewModel addGameViewModel)
        {
            this.addGameViewModel = addGameViewModel;
        }

        public override void Execute(object parameter)
        {
            var item = new ItemViewModel
            {
                ItemType = ItemTypes.BigBox
            };

            if (parameter is ItemType itemType)
            {
                item.ItemType = itemType;
            }

            addGameViewModel.GameItems.Add(item);
            addGameViewModel.CurrentGameItem = item;
        }
    }
}