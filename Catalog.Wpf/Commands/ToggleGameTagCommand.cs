using System.Linq;
using System.Windows;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class ToggleTagCommand : CommandBase
    {
        private readonly CatalogContext context;
        private readonly MainWindowViewModel viewModel;

        public ToggleTagCommand(CatalogContext context, MainWindowViewModel viewModel)
        {
            this.context = context;
            this.viewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is Tag tag))
            {
                return;
            }

            var game = ((GameViewModel) viewModel.FilteredGames.CurrentItem).GameCopy;

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

            context.SaveChanges();

            // TODO: Refresh just the relevant game.
            viewModel.RefreshGamesCollection();
        }
    }
}
