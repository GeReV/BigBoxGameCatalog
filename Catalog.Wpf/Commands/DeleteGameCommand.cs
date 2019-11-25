using System.Collections.Generic;
using System.IO;
using System.Windows;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using File = Catalog.Model.File;

namespace Catalog.Wpf.Commands
{
    public class DeleteGameCommand : CommandBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public DeleteGameCommand(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is GameViewModel game))
            {
                return;
            }

            var messageResult = MessageBox.Show(
                $"Are you sure you want to delete this copy of {game.Title}?",
                "Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation
            );

            if (messageResult != MessageBoxResult.Yes)
            {
                return;
            }

            Application.Current.Database().Remove(game.GameCopy);

            mainWindowViewModel.RefreshGamesCollection();
        }
    }
}
