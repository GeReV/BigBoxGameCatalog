using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public sealed class SaveGameArguments
        {
            public GameCopy Game { get; }
            public ICollection<ScreenshotViewModel> GameScreenshots { get; }
            public ScreenshotViewModel? GameCoverImage { get; }
            public IProgress<int>? Progress { get; }

            public SaveGameArguments(GameCopy game, ICollection<ScreenshotViewModel> gameScreenshots,
                ScreenshotViewModel? gameCoverImage, IProgress<int>? progress = null)
            {
                Game = game;
                GameScreenshots = gameScreenshots;
                GameCoverImage = gameCoverImage;
                Progress = progress;
            }
        }

        protected override async Task Perform(object? parameter)
        {
            if (!(parameter is SaveGameArguments args))
            {
                return;
            }

            var game = args.Game;

            await using var database = Application.Current.Database();

            database.Attach(game);

            await PersistGame(database);

            var destinationDirectory = BuildGamePath(game);

            var screenshots = await DownloadScreenshots(
                Path.Combine(destinationDirectory, "screenshots"),
                args.GameScreenshots,
                args.Progress
            );

            var cover = await DownloadCoverArt(destinationDirectory, args.GameCoverImage);

            game.CoverImage = cover;
            game.Screenshots = screenshots.Distinct().ToList();

            await PersistGame(database);
        }

        private static async Task PersistGame(DbContext db)
        {
            await using var transaction = await db.Database.BeginTransactionAsync();

            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        private static string BuildGamePath(GameCopy gameCopy)
        {
            var directoryName = $"{gameCopy.GameCopyId:D4}";

            if (!string.IsNullOrWhiteSpace(gameCopy.MobyGamesSlug))
            {
                directoryName += $"-{gameCopy.MobyGamesSlug}";
            }

            return Path.Combine(Application.Current.HomeDirectory(), directoryName);
        }

        private static async Task<IEnumerable<string>> DownloadScreenshots(string destinationDirectory,
            ICollection<ScreenshotViewModel> selectedScreenshots, IProgress<int> progress = null)
        {
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
                    progress
                );

            return newScreenshots
                .Select(HomeDirectoryHelpers.ToRelativePath)
                .Concat(existingScreenshots)
                .OrderBy(s => s);
        }

        private static async Task<string?> DownloadCoverArt(string destinationDirectory, ScreenshotViewModel? gameCoverImage)
        {
            if (gameCoverImage == null)
            {
                return null;
            }

            var url = gameCoverImage.Url;

            if (new Uri(url).IsFile)
            {
                return url;
            }

            var path = await new ImageDownloader(Application.Current.ScraperWebClient())
                .DownloadCoverArt(destinationDirectory, url);

            return HomeDirectoryHelpers.ToRelativePath(path);
        }
    }
}
