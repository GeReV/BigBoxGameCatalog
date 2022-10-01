using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.Extensions;
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

        protected override bool CanExecuteImpl(object? parameter) =>
            !string.IsNullOrEmpty(editGameViewModel.GameMobyGamesSlug);

        protected override async Task Perform(object? parameter)
        {
            editGameViewModel.Status = EditGameViewModel.ViewStatus.Searching;

            try
            {
                var scraper = new Scraper(Application.Current.ScraperWebClient());

                Debug.Assert(
                    editGameViewModel.GameMobyGamesSlug != null,
                    "editGameViewModel.GameMobyGamesSlug != null"
                );

                var covers = await Task.Run(() => scraper.GetCoverArt(editGameViewModel.GameMobyGamesSlug));

                var selectionDialog = new CoverSelectionDialog(covers)
                {
                    Owner = editGameViewModel.ParentWindow
                };

                if (selectionDialog.ShowDialog() == true)
                {
                    editGameViewModel.GameCoverImage = new ScreenshotViewModel(
                        selectionDialog.SelectedResult.Thumbnail,
                        selectionDialog.SelectedResult.Url
                    );
                }

                editGameViewModel.Status = EditGameViewModel.ViewStatus.Idle;
            }
            catch (Exception e)
            {
                editGameViewModel.CurrentException = e;
                editGameViewModel.Status = EditGameViewModel.ViewStatus.Error;
            }
        }
    }
}
