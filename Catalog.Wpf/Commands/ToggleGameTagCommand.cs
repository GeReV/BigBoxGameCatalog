using System.Linq;
using System.Windows;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class ToggleTagCommand : CommandBase
    {
        private readonly MainWindowViewModel viewModel;

        public ToggleTagCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return viewModel.FilteredGames.CurrentItem != null;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is Tag tag))
            {
                return;
            }
            
            var originalGame = ((GameViewModel) viewModel.FilteredGames.CurrentItem).GameCopy;

            using var db = Application.Current.Database();

            var game = db.Games.Find(originalGame.GameCopyId);

            var gameCopyTag = game.GameCopyTags.FirstOrDefault(gct => gct.TagId == tag.TagId);

            if (gameCopyTag == null)
            {
                game.GameCopyTags.Add(new GameCopyTag
                {
                    Game = game,
                    Tag = tag
                });
            }
            else
            {
                game.GameCopyTags.Remove(gameCopyTag);
            }

            db.SaveChanges();

            // TODO: Refresh just the relevant game.
            viewModel.RefreshGamesCollection();
        }
    }
}
