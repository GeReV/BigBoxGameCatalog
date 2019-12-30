using System.Windows;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class DeleteGameCommand : CommandBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public DeleteGameCommand(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is GameViewModel;
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

            using var database = Application.Current.Database();

            database.Remove(game.GameCopy);

            database.SaveChanges();

            mainWindowViewModel.RefreshGamesCollection();
        }
    }
}
