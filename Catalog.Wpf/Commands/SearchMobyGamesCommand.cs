﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Catalog.Model;
using Catalog.Scrapers;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class SearchMobyGamesCommand : AsyncCommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        // TODO: Turn into an application-level option?
        private static string[] PlatformPriorities = { "Windows", "DOS" };

        public SearchMobyGamesCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        protected override bool CanExecuteImpl(object? parameter) =>
            !string.IsNullOrWhiteSpace(parameter as string ?? string.Empty);

        protected override async Task Perform(object? parameter)
        {
            editGameViewModel.Status = EditGameViewModel.ViewStatus.Searching;

            try
            {
                await SearchMobyGames(parameter as string ?? string.Empty);

                editGameViewModel.Status = EditGameViewModel.ViewStatus.Idle;
            }
            catch (Exception e)
            {
                editGameViewModel.Status = EditGameViewModel.ViewStatus.Error;
                editGameViewModel.CurrentException = e;
            }
        }

        private async Task SearchMobyGames(string term)
        {
            var scraper = new Scraper(Application.Current.ScraperWebClient());

            var entries = await Task.Run(() => scraper.Search(term));

            var disambiguationDialog = new GameDisambiguationDialog(entries)
            {
                Owner = editGameViewModel.ParentWindow
            };

            if (disambiguationDialog.ShowDialog() != true)
            {
                return;
            }

            var url = disambiguationDialog.SelectedResult.Url;

            if (disambiguationDialog.SelectedResult.Releases.Any())
            {
                // Find the first release that matches our preferred platforms.
                foreach (var platform in PlatformPriorities)
                {
                    var matchingRelease =
                        disambiguationDialog.SelectedResult.Releases
                            .FirstOrDefault(release => release.Platform.Equals(platform, StringComparison.OrdinalIgnoreCase));

                    if (matchingRelease == null || string.IsNullOrEmpty(matchingRelease.Url))
                    {
                        continue;
                    }

                    url = matchingRelease.Url;
                }
            }

            var gameEntry = scraper.GetGame(url);

            GetSpecs(gameEntry);

            GetScreenshots(gameEntry);

            editGameViewModel.GameScreenshots.Clear();

            editGameViewModel.Title = gameEntry.Name;
            editGameViewModel.GameMobyGamesSlug = gameEntry.Slug;
            editGameViewModel.GameLinks.Add(gameEntry.Url);

            var publisher = editGameViewModel.Publishers.ToList().Find(p => p.Slug == gameEntry.Publisher.Slug);

            if (publisher == null)
            {
                publisher = new Publisher
                {
                    Name = gameEntry.Publisher.Name,
                    Slug = gameEntry.Publisher.Slug,
                    Links = new List<string> {gameEntry.Publisher.Url}
                };

                editGameViewModel.Publishers.Add(publisher);
            }

            editGameViewModel.GamePublisher = publisher;

            var developerCollection = editGameViewModel.Developers.ToList();

            foreach (var devEntry in gameEntry.Developers)
            {
                var developer = developerCollection.Find(d => d.Slug == devEntry.Slug);

                if (developer == null)
                {
                    developer = new Developer
                    {
                        Name = devEntry.Name,
                        Slug = devEntry.Slug,
                        Links = new List<string> {devEntry.Url},
                    };

                    editGameViewModel.Developers.Add(developer);
                }

                editGameViewModel.GameDevelopers.Add(developer);
            }
        }

        private async void GetSpecs(GameEntry gameEntry)
        {
            var specs = await Task.Run(() =>
                new Scraper(Application.Current.ScraperWebClient()).GetGameSpecs(gameEntry.Slug));

            var platforms = Enum
                .GetValues(typeof(Platform))
                .Cast<Platform>()
                .Where(platform => specs.Platforms.Contains(platform.GetDescription()))
                .ToList();

            foreach (var platform in platforms)
            {
                editGameViewModel.GamePlatforms.Add(platform);
            }
        }

        private async void GetScreenshots(GameEntry gameEntry)
        {
            IEnumerable<ScreenshotEntry> screenshotEntries = await Task.Run(() =>
                new Scraper(Application.Current.ScraperWebClient())
                    .GetGameScreenshots(gameEntry.Slug)
            );

            // TODO: Get this from config.
            var listItems = screenshotEntries.Take(10).ToList();

            var images = listItems
                .Select(item => new ScreenshotViewModel(item.Thumbnail, item.Url));

            foreach (var image in images)
            {
                editGameViewModel.GameScreenshots.Add(image);
            }
        }
    }
}
