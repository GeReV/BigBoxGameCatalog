using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class SaveGameCommand : AsyncCommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        public SaveGameCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        protected override async Task Perform(object parameter)
        {
            editGameViewModel.Status = EditGameViewModel.ViewStatus.DownloadingScreenshots;

            var gameTask = await BuildGame();

            await InsertGame(gameTask);

            editGameViewModel.Status = EditGameViewModel.ViewStatus.Idle;
        }

        private static async Task InsertGame(GameCopy game)
        {
            var db = Application.Current.Database();

            using var transaction = await db.Database.BeginTransactionAsync();

            db.Update(game);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        private async Task<IEnumerable<string>> DownloadScreenshots()
        {
            var destinationDirectory = Path.Combine(
                Application.Current.HomeDirectory(),
                editGameViewModel.GameMobyGamesSlug,
                "screenshots"
            );

            var selectedScreenshots = editGameViewModel.GameScreenshots;

            var screenshotsToDownload = selectedScreenshots
                .Where(ss => !new Uri(ss.Url).IsFile)
                .ToList();

            var existingScreenshots = selectedScreenshots
                .Except(screenshotsToDownload)
                .Select(ss => ss.Url);

            var newScreenshots = await new ImageDownloader(Application.Current.ScraperWebClient())
                .DownloadScreenshots(
                    destinationDirectory,
                    screenshotsToDownload.Select(ss => ss.Url),
                    new Progress<int>(percentage => editGameViewModel.SaveProgress = percentage)
                );

            return newScreenshots
                .Concat(existingScreenshots)
                .OrderBy(s => s);
        }

        private async Task<string> DownloadCoverArt()
        {
            var gameDirectory =
                Path.Combine(Application.Current.HomeDirectory(), editGameViewModel.GameMobyGamesSlug);

            if (editGameViewModel.GameCoverImage == null)
            {
                return null;
            }

            var url = editGameViewModel.GameCoverImage.Url;

            if (!new Uri(url).IsFile)
            {
                return await new ImageDownloader(Application.Current.ScraperWebClient())
                    .DownloadCoverArt(gameDirectory, url);
            }

            return url;
        }

        private async Task<GameCopy> BuildGame()
        {
            var screenshots = await DownloadScreenshots();

            var cover = await DownloadCoverArt();

            var gameCopy = editGameViewModel.Game;

            gameCopy.Title = editGameViewModel.GameTitle;
            gameCopy.Sealed = editGameViewModel.GameSealed;
            gameCopy.MobyGamesSlug = editGameViewModel.GameMobyGamesSlug;
            gameCopy.Platforms = editGameViewModel.GamePlatforms.Distinct().ToList();
            gameCopy.Publisher = editGameViewModel.GamePublisher;
            gameCopy.GameCopyDevelopers = editGameViewModel.GameDevelopers.Distinct()
                .Select(dev => new GameCopyDeveloper
                {
                    Developer = dev,
                    Game = gameCopy
                })
                .ToList();
            gameCopy.Items = editGameViewModel.GameItems.Select(item => item.BuildItem()).ToList();
            gameCopy.Links = editGameViewModel.GameLinks.Distinct().ToList();
            gameCopy.Notes = editGameViewModel.GameNotes;
            gameCopy.TwoLetterIsoLanguageName =
                editGameViewModel.GameLanguages.Select(ci => ci.TwoLetterISOLanguageName).Distinct().ToList();
            gameCopy.ReleaseDate = editGameViewModel.GameReleaseDate;
            gameCopy.CoverImage = cover;
            gameCopy.Screenshots = screenshots.Distinct().ToList();

            return gameCopy;
        }
    }
}
