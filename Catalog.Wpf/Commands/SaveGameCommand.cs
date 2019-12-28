using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.Commands
{
    public class SaveGameCommand : AsyncCommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        public SaveGameCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        protected override async Task Perform(object? parameter)
        {
            editGameViewModel.Status = EditGameViewModel.ViewStatus.DownloadingScreenshots;

            await using var database = Application.Current.Database();

            var gameCopy = editGameViewModel.Game;

            database.Attach(gameCopy);

            SetGameData(gameCopy);

            await PersistGame(database, gameCopy);

            await DownloadGameAssets(database, gameCopy);

            editGameViewModel.Status = EditGameViewModel.ViewStatus.Idle;
        }

        private static async Task PersistGame(DbContext db, GameCopy game)
        {
            await using var transaction = await db.Database.BeginTransactionAsync();

            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        private async Task DownloadGameAssets(DbContext database, GameCopy gameCopy)
        {
            if (gameCopy.IsNew)
            {
                throw new ArgumentException("Provided GameCopy must be persisted before downloading game assets", nameof(gameCopy));
            }

            var screenshots = await DownloadScreenshots();

            var cover = await DownloadCoverArt();

            gameCopy.CoverImage = cover;
            gameCopy.Screenshots = screenshots.Distinct().ToList();

            await PersistGame(database, gameCopy);
        }

        private string BuildGamePath()
        {
            var directoryName = $"{editGameViewModel.Game.GameCopyId:D4}";

            if (!string.IsNullOrWhiteSpace(editGameViewModel.GameMobyGamesSlug))
            {
                directoryName += $"-{editGameViewModel.GameMobyGamesSlug}";
            }

            return Path.Combine(Application.Current.HomeDirectory(), directoryName);
        }

        private async Task<IEnumerable<string>> DownloadScreenshots()
        {
            var destinationDirectory = Path.Combine(BuildGamePath(), "screenshots");

            var selectedScreenshots = editGameViewModel.GameScreenshots;

            var screenshotsToDownload = selectedScreenshots
                .Where(ss => Uri.TryCreate(ss.Url, UriKind.Absolute, out var uri) && !uri.IsFile)
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
                .Select(HomeDirectoryHelpers.ToRelativePath)
                .Concat(existingScreenshots)
                .OrderBy(s => s);
        }

        private async Task<string?> DownloadCoverArt()
        {
            var gameDirectory = BuildGamePath();

            if (editGameViewModel.GameCoverImage == null)
            {
                return null;
            }

            var url = editGameViewModel.GameCoverImage.Url;

            if (new Uri(url).IsFile)
            {
                return url;
            }

            var path = await new ImageDownloader(Application.Current.ScraperWebClient())
                .DownloadCoverArt(gameDirectory, url);

            return HomeDirectoryHelpers.ToRelativePath(path);
        }

        private void SetGameData(GameCopy gameCopy)
        {
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
        }
    }
}
