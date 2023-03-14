using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Wpf.Extensions;
using Catalog.Wpf.Helpers;
using Catalog.Wpf.ViewModel;
using MobyGames.API.Exceptions;
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
            editGameViewModel.MobyGameId is not null;

        protected override async Task Perform(object? parameter)
        {
            editGameViewModel.Status = EditGameViewModel.ViewStatus.Searching;

            try
            {
                var client = Application.Current.MobyGamesClient();

                Debug.Assert(
                    editGameViewModel.MobyGameId != null,
                    "editGameViewModel.MobyGameId != null"
                );

                // Game object may be null if we loaded an existing game.
                var mobyGame = editGameViewModel.MobyGame ?? await client.Game(editGameViewModel.MobyGameId.Value);

                var selectedPlatform = GamePlatformSelector.SelectPreferredPlatform(mobyGame);

                var covers = (await client.GameCovers(mobyGame.Id, selectedPlatform.Id)).ToList();

                if (covers.Count == 0)
                {
                    MessageBox.Show(
                        "No covers were found for this game",
                        "No Results",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    return;
                }

                var selectionDialog = new CoverSelectionDialog(covers)
                {
                    Owner = editGameViewModel.ParentWindow
                };

                if (selectionDialog.ShowDialog() == true)
                {
                    editGameViewModel.GameCoverImage = new ScreenshotViewModel(
                        selectionDialog.SelectedResult.ThumbnailImage,
                        selectionDialog.SelectedResult.Image
                    );
                }

                editGameViewModel.Status = EditGameViewModel.ViewStatus.Idle;
            }
            catch (MobyGamesMissingApiKeyException)
            {
                MessageBox.Show(
                    "Application is missing MobyGames API key. Please add you API key to the App.config file and restart application.",
                    "MobyGames API key missing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            catch (Exception e)
            {
                editGameViewModel.CurrentException = e;
                editGameViewModel.Status = EditGameViewModel.ViewStatus.Error;
            }
        }
    }
}
