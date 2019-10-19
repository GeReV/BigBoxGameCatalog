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

            db.GetGamesCollection().Insert(game);
        }

        private async Task<IEnumerable<Image>> DownloadScreenshots()
        {
            var screenshotDirectory = Path.Combine(Application.Current.HomeDirectory(), "screenshots",
                editGameViewModel.GameMobyGamesSlug);

            return await new ScreenshotDownloader(new CachingWebClient())
                .DownloadScreenshots(
                    screenshotDirectory,
                    editGameViewModel.GameSelectedScreenshots
                        .Select(ss => ((ScreenshotViewModel)ss).Url),
                    new Progress<int>(percentage => editGameViewModel.SaveProgress = percentage)
                );
        }

        private async Task<GameCopy> BuildGame()
        {
            var screenshots = await DownloadScreenshots();

            return new GameCopy
            {
                Title = editGameViewModel.GameTitle,
                MobyGamesSlug = editGameViewModel.GameMobyGamesSlug,
                Platforms = editGameViewModel.GamePlatforms.ToList(),
                Publisher = editGameViewModel.GamePublisher,
                Developers = editGameViewModel.GameDevelopers.ToList(),
                Items = editGameViewModel.GameItems.Select(item => item.BuildItem()).ToList(),
                Links = editGameViewModel.GameLinks.ToList(),
                Notes = editGameViewModel.GameNotes,
                TwoLetterIsoLanguageName = editGameViewModel.GameTwoLetterIsoLanguageName.ToList(),
                ReleaseDate = editGameViewModel.GameReleaseDate,
                Screenshots = screenshots.ToList()
            };
        }
    }
}