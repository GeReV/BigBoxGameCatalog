using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.Extensions;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.Commands
{
    public class SaveGameCommand : AsyncCommandBase
    {
        public sealed class SaveGameArguments
        {
            public int GameId { get; }
            public EditGameViewModel EditGameViewModel { get; }
            public IProgress<int>? Progress { get; }

            public SaveGameArguments(int gameId, EditGameViewModel editGameViewModel, IProgress<int>? progress = null)
            {
                GameId = gameId;
                EditGameViewModel = editGameViewModel;
                Progress = progress;
            }
        }

        protected override async Task Perform(object? parameter)
        {
            if (parameter is not SaveGameArguments args)
            {
                return;
            }

            args.EditGameViewModel.Status = EditGameViewModel.ViewStatus.DownloadingScreenshots;

            try
            {
                await using var database = Application.Current.Database();

                var game = args.GameId == 0 ? new GameCopy() : GamesRepository.LoadGame(database, args.GameId);

                if (game.IsNew)
                {
                    database.Add(game);
                }

                UpdateGame(game, args.EditGameViewModel);

                await PersistGame(database);

                var destinationDirectory = BuildGamePath(game);

                var screenshots = await DownloadScreenshots(
                    Path.Combine(destinationDirectory, "screenshots"),
                    args.EditGameViewModel.GameScreenshots,
                    args.Progress
                );

                var cover = await DownloadCoverArt(destinationDirectory, args.EditGameViewModel.GameCoverImage);

                game.CoverImage = cover;
                game.Screenshots = screenshots.Distinct().ToList();

                await PersistGame(database);

                args.EditGameViewModel.Status = EditGameViewModel.ViewStatus.Idle;
            }
            catch (Exception e)
            {
                args.EditGameViewModel.CurrentException = e;
                args.EditGameViewModel.Status = EditGameViewModel.ViewStatus.Error;

                throw;
            }
        }

        private static void UpdateGameCopyDevelopers(
            ICollection<GameCopyDeveloper> gameDevelopers,
            IEnumerable<Developer> nextGameDevelopers
        )
        {
            var currentGameDeveloperIds = gameDevelopers
                .Select(gcd => gcd.DeveloperId)
                .ToImmutableHashSet();

            var nextGameDeveloperIds = gameDevelopers
                .Select(gd => gd.DeveloperId)
                .ToImmutableHashSet();

            var dropGameDevelopers =
                gameDevelopers.Where(gcd => !nextGameDeveloperIds.Contains(gcd.DeveloperId)).ToList();

            foreach (var dropGameDeveloper in dropGameDevelopers)
            {
                gameDevelopers.Remove(dropGameDeveloper);
            }

            var addGameDevelopers = nextGameDevelopers
                .Where(gd => !currentGameDeveloperIds.Contains(gd.DeveloperId))
                .Select(
                    dev => new GameCopyDeveloper
                    {
                        DeveloperId = dev.DeveloperId,
                        Developer = dev
                    }
                );

            foreach (var addGameDeveloper in addGameDevelopers)
            {
                gameDevelopers.Add(addGameDeveloper);
            }
        }

        private static void UpdateGameItems(ICollection<GameItem> gameItems, ICollection<ItemViewModel> nextGameItems)
        {
            var currentGameItemIds = gameItems
                .Select(gcd => gcd.GameItemId)
                .ToImmutableHashSet();

            var nextGameItemIds = nextGameItems
                .Select(gd => gd.ItemId)
                .ToImmutableHashSet();

            var dropGameItems =
                gameItems.Where(gcd => !nextGameItemIds.Contains(gcd.GameItemId)).ToList();

            foreach (var dropGameItem in dropGameItems)
            {
                gameItems.Remove(dropGameItem);
            }

            foreach (var gameItem in gameItems)
            {
                var matchingNextItem = nextGameItems.First(gi => gi.ItemId == gameItem.GameItemId);

                gameItem.CopyFrom(matchingNextItem.BuildItem());
            }

            var addGameItems = nextGameItems
                .Where(gd => !currentGameItemIds.Contains(gd.ItemId))
                .Select(item => item.BuildItem());

            foreach (var addGameItem in addGameItems)
            {
                gameItems.Add(addGameItem);
            }
        }

        private static void UpdateGame(GameCopy game, EditGameViewModel editGameViewModel)
        {
            game.Title = editGameViewModel.Title;
            game.Sealed = editGameViewModel.GameSealed;
            game.MobyGamesSlug = editGameViewModel.GameMobyGamesSlug ?? string.Empty;
            game.Platforms = editGameViewModel.GamePlatforms.Distinct().ToList();

            if (editGameViewModel.GamePublisher != null && (editGameViewModel.GamePublisher.IsNew ||
                                                            editGameViewModel.GamePublisher.PublisherId !=
                                                            game.PublisherId))
            {
                game.Publisher = editGameViewModel.GamePublisher;
            }

            game.Links = editGameViewModel.GameLinks.Distinct().ToList();
            game.Notes = editGameViewModel.GameNotes;
            game.TwoLetterIsoLanguageName =
                editGameViewModel.GameLanguages.Select(ci => ci.TwoLetterISOLanguageName).Distinct().ToList();
            game.ReleaseDate = editGameViewModel.GameReleaseDate;

            UpdateGameCopyDevelopers(game.GameCopyDevelopers, editGameViewModel.GameDevelopers);

            UpdateGameItems(game.Items, editGameViewModel.GameItems);
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

        private static async Task<IEnumerable<string>> DownloadScreenshots(
            string destinationDirectory,
            ICollection<ScreenshotViewModel> selectedScreenshots,
            IProgress<int>? progress = null
        )
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
                .Select(HomeDirectoryExtensions.ToRelativePath)
                .Concat(existingScreenshots)
                .OrderBy(s => s);
        }

        private static async Task<string?> DownloadCoverArt(
            string destinationDirectory,
            ScreenshotViewModel? gameCoverImage
        )
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

            return HomeDirectoryExtensions.ToRelativePath(path);
        }
    }
}
