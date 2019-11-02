using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Model;
using Catalog.Scrapers;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.ViewModel;
using Xceed.Wpf.Toolkit.Core.Converters;
using File = System.IO.File;

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

            InsertGame(gameTask);

            editGameViewModel.Status = EditGameViewModel.ViewStatus.Idle;
        }

        private static void InsertGame(GameCopy game)
        {
            var db = Application.Current.Database();

            if (game.Publisher.PublisherId == 0)
            {
                db.GetPublishersCollection().Insert(game.Publisher);
            }

            var developersCollection = db.GetDevelopersCollection();

            foreach (var developer in game.Developers.Where(d => d.DeveloperId == 0))
            {
                developersCollection.Insert(developer);
            }

            db.GetGamesCollection().Upsert(game);
        }

        private async Task<IEnumerable<Image>> DownloadScreenshots()
        {
            var destinationDirectory = Path.Combine(
                Application.Current.HomeDirectory(),
                editGameViewModel.GameMobyGamesSlug,
                "screenshots"
            );

            var selectedScreenshots = editGameViewModel.GameSelectedScreenshots;

            var screenshotsToDownload = selectedScreenshots
                .Where(ss => !new Uri(ss.Url).IsFile)
                .ToList();

            var existingScreenshots = selectedScreenshots
                .Except(screenshotsToDownload)
                .Select(ss => new Image(ss.Url));

            var newScreenshots = await new ImageDownloader(Application.Current.ScraperWebClient())
                .DownloadScreenshots(
                    destinationDirectory,
                    screenshotsToDownload.Select(ss => ss.Url),
                    new Progress<int>(percentage => editGameViewModel.SaveProgress = percentage)
                );

            return newScreenshots
                .Concat(existingScreenshots)
                .OrderBy(image => image.Path);
        }

        private async Task<Image> DownloadCoverArt()
        {
            var gameDirectory =
                Path.Combine(Application.Current.HomeDirectory(), editGameViewModel.GameMobyGamesSlug);

            var url = editGameViewModel.GameCoverImage.Url;

            if (!new Uri(url).IsFile)
            {
                return await new ImageDownloader(Application.Current.ScraperWebClient())
                    .DownloadCoverArt(gameDirectory, url);
            }

            return new Image(url);
        }

        private async Task<GameCopy> BuildGame()
        {
            var screenshots = await DownloadScreenshots();

            var cover = await DownloadCoverArt();

            return new GameCopy
            {
                GameCopyId = editGameViewModel.GameId,
                Title = editGameViewModel.GameTitle,
                Sealed = editGameViewModel.GameSealed,
                MobyGamesSlug = editGameViewModel.GameMobyGamesSlug,
                Platforms = editGameViewModel.GamePlatforms.Distinct().ToList(),
                Publisher = editGameViewModel.GamePublisher,
                Developers = editGameViewModel.GameDevelopers.Distinct().ToList(),
                Items = editGameViewModel.GameItems.Select(item => item.BuildItem()).ToList(),
                Links = editGameViewModel.GameLinks.Distinct().ToList(),
                Notes = editGameViewModel.GameNotes,
                TwoLetterIsoLanguageName =
                    editGameViewModel.GameLanguages.Select(ci => ci.TwoLetterISOLanguageName).Distinct().ToList(),
                ReleaseDate = editGameViewModel.GameReleaseDate,
                CoverImage = cover,
                Screenshots = screenshots.Distinct().ToList()
            };
        }
    }
}