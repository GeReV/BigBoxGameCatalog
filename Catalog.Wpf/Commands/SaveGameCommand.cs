using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.ViewModel;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace Catalog.Wpf.Commands
{
    public class SaveGameCommand : AsyncCommandBase
    {
        private readonly AddGameViewModel addGameViewModel;

        public SaveGameCommand(AddGameViewModel addGameViewModel)
        {
            this.addGameViewModel = addGameViewModel;
        }

        protected override async Task Perform(object parameter)
        {
            addGameViewModel.Status = AddGameViewModel.ViewStatus.DownloadingScreenshots;

            var gameTask = await BuildGame();

            InsertGame(gameTask);

            addGameViewModel.Status = AddGameViewModel.ViewStatus.Idle;
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
                addGameViewModel.GameMobyGamesSlug);

            return await ScreenshotDownloader.DownloadScreenshots(
                screenshotDirectory,
                addGameViewModel.GameSelectedScreenshots
                    .Select(ss => ss.Url),
                new Progress<int>(percentage => addGameViewModel.SaveProgress = percentage)
            );
        }

        private async Task<GameCopy> BuildGame()
        {
            var screenshots = await DownloadScreenshots();

            return new GameCopy
            {
                Title = addGameViewModel.GameTitle,
                MobyGamesSlug = addGameViewModel.GameMobyGamesSlug,
                Platforms = addGameViewModel.GamePlatforms.ToList(),
                Publisher = addGameViewModel.GamePublisher,
                Developers = addGameViewModel.GameDevelopers.ToList(),
                Items = addGameViewModel.GameItems.Select(item => item.BuildItem()).ToList(),
                Links = addGameViewModel.GameLinks.ToList(),
                Notes = addGameViewModel.GameNotes,
                TwoLetterIsoLanguageName = addGameViewModel.GameTwoLetterIsoLanguageName.ToList(),
                ReleaseDate = addGameViewModel.GameReleaseDate,
                Screenshots = screenshots.ToList()
            };
        }
    }
}