using System.Linq;
using System.Windows;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

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

            using var db = Application.Current.Database();

            db.Entry(game.GameCopy)
                .Collection(v => v.Items)
                .Load();

            var editGameDialog = new EditGameDialog(game.GameCopy)
            {
                Owner = Application.Current.MainWindow
            };

            if (editGameDialog.ShowDialog() == true)
            {
                mainWindowViewModel.RefreshGamesCollection();
            }
        }
    }
}
