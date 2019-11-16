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

            DeleteImages(game.GameCopy.Screenshots);

            foreach (var item in game.GameCopy.Items)
            {
                DeleteFiles(item.Files);

                DeleteImages(item.Scans);
            }

            Application.Current.Database().GetGamesCollection().Delete(game.GameCopy.GameCopyId);

            mainWindowViewModel.RefreshGamesCollection();
        }

        private static void DeleteFiles(IEnumerable<File> files)
        {
            var filesCollection = Application.Current.Database().GetFilesCollection();

            foreach (var file in files)
            {
                filesCollection.Delete(file.LocalResourceId);
            }
        }

        private static void DeleteImages(IEnumerable<Image> images)
        {
            var imagesCollection = Application.Current.Database().GetImagesCollection();

            foreach (var image in images)
            {
                imagesCollection.Delete(image.LocalResourceId);
            }
        }
    }
}
