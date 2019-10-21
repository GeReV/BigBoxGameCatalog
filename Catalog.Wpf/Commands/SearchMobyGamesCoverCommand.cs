using System.Threading.Tasks;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.ViewModel;
using Application = System.Windows.Application;

namespace Catalog.Wpf.Commands
{
    public class SearchMobyGamesCoverCommand : AsyncCommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        public SearchMobyGamesCoverCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        protected override bool CanExecuteImpl(object parameter) =>
            !string.IsNullOrEmpty(editGameViewModel.GameMobyGamesSlug);

        protected override async Task Perform(object parameter)
        {
            var scraper = new Scraper(Application.Current.ScraperWebClient());

            var covers = await Task.Run(() => scraper.GetCoverArt(editGameViewModel.GameMobyGamesSlug));

            var selectionDialog = new CoverSelectionDialog(covers);

            if (selectionDialog.ShowDialog() == true)
            {
                editGameViewModel.GameCoverImage = new ScreenshotViewModel
                {
                    Url = selectionDialog.SelectedResult.Url,
                    ThumbnailUrl = selectionDialog.SelectedResult.Thumbnail
                };
            }
        }
    }
}