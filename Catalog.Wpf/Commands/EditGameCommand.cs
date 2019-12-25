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

            db.Attach(game.GameCopy)
                .Collection(v => v.Items)
                .Query()
                .Include(item => item.Files)
                .Include(item => item.Scans)
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
