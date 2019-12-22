using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.Commands
{
    public class ToggleTagCommand : AsyncCommandBase
    {
        private readonly MainWindowViewModel viewModel;

        public ToggleTagCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        protected override bool CanExecuteImpl(object parameter)
        {
            return parameter is object[] parameters &&
                   parameters[1] is object[] selectedItems &&
                   selectedItems.Any();
        }

        protected override async Task Perform(object parameter)
        {
            if (!(parameter is object[] parameters))
            {
                return;
            }

            if (!(parameters[0] is Tag tag && parameters[1] is object[] selectedItems))
            {
                return;
            }

            var gameIds = selectedItems.Cast<GameViewModel>().Select(item => item.GameCopy.GameCopyId);

            await using var db = Application.Current.Database();

            var games = await db.Games
                .Include(game => game.GameCopyTags)
                .Where(game => gameIds.Contains(game.GameCopyId))
                .ToListAsync();

            var gamesMissingTag = games
                .Where(game => game.GameCopyTags.FirstOrDefault(gct => gct.TagId == tag.TagId) == null)
                .ToList();

            if (gamesMissingTag.Any())
            {
                foreach (var game in gamesMissingTag)
                {
                    game.GameCopyTags.Add(new GameCopyTag
                    {
                        GameCopyId = game.GameCopyId,
                        TagId = tag.TagId
                    });
                }
            }
            else
            {
                foreach (var game in games)
                {
                    var gameCopyTag = game.GameCopyTags.FirstOrDefault(gct => gct.TagId == tag.TagId);

                    game.GameCopyTags.Remove(gameCopyTag);
                }
            }

            await db.SaveChangesAsync();

            // TODO: Refresh just the relevant games.
            viewModel.RefreshGamesCollection();
        }
    }
}
