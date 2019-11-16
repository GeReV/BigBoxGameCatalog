using System.Windows;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class EditGameCommand : CommandBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public EditGameCommand(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is GameViewModel game))
            {
                return;
            }

            var db = Application.Current.Database();

            var gamesCollection = db.GetGamesCollection();

            var gameCopy = gamesCollection
                .IncludeAll(2)
                .FindById(game.GameCopy.GameCopyId);

            var viewModel = EditGameViewModel.FromGameCopy(gameCopy, db);

            if (new EditGameDialog(viewModel).ShowDialog() == true)
            {
                mainWindowViewModel.RefreshGamesCollection();
            }
        }
    }
}
