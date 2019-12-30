using System.Windows;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.Commands
{
    public class DuplicateGameCommand : CommandBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public DuplicateGameCommand(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is int;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is int gameCopyId))
            {
                return;
            }

            using var database = Application.Current.Database();

            var gameCopy = LoadGame(database, gameCopyId);

            var duplicate = gameCopy.Clone();

            database.Add(duplicate);

            database.SaveChanges();

            mainWindowViewModel.RefreshGame(duplicate.GameCopyId);
        }

        private static GameCopy LoadGame(CatalogContext database, int gameCopyId)
        {
            var gameCopy = database.Games.Find(gameCopyId);

            var entry = database.Entry(gameCopy);

            entry
                .Collection(g => g.Items)
                .Query()
                .Include(item => item.Files)
                .Include(item => item.Scans)
                .Load();

            entry
                .Collection(g => g.GameCopyDevelopers)
                .Load();

            entry
                .Collection(g => g.GameCopyTags)
                .Load();

            entry
                .Reference(g => g.Publisher)
                .Load();

            return gameCopy;
        }
    }
}
