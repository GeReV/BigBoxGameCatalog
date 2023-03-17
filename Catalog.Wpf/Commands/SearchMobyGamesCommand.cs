using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Model;
using Catalog.Wpf.Extensions;
using Catalog.Wpf.Helpers;
using Catalog.Wpf.ViewModel;
using MobyGames.API;
using MobyGames.API.DataObjects;
using MobyGames.API.Exceptions;
using Platform = Catalog.Model.Platform;

namespace Catalog.Wpf.Commands
{
    public class SearchMobyGamesCommand : AsyncCommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        private Uri mobyGamesBaseUri = new("https://www.mobygames.com");

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
            try
            {
                var client = Application.Current.MobyGamesClient();

                var entries = (await client.Games(new MobyGamesClientOptions.GamesOptions { Title = term }))
                    .ToList();

                if (entries.Count == 0)
                {
                    MessageBox.Show(
                        $"No results were found for \"{term}\"",
                        "No Results",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    return;
                }

                Game gameEntry;

                if (entries.Count > 1)
                {
                    var disambiguationDialog = new GameDisambiguationDialog(entries)
                    {
                        Owner = editGameViewModel.ParentWindow
                    };

                    if (disambiguationDialog.ShowDialog() != true)
                    {
                        return;
                    }

                    gameEntry = disambiguationDialog.SelectedResult;
                }
                else
                {
                    gameEntry = entries[0];
                }

                var preferredPlatform = GamePlatformSelector.SelectPreferredPlatform(gameEntry);

                GetSpecs(gameEntry);

                editGameViewModel.GameScreenshots.Clear();

                editGameViewModel.Title = gameEntry.Title;
                editGameViewModel.MobyGame = gameEntry;

                if (gameEntry.MobyUrl is not null)
                {
                    editGameViewModel.GameLinks.Add(gameEntry.MobyUrl.ToString());
                }

                var gamePlatform = await client.GamePlatform(gameEntry.Id, preferredPlatform.Id);

                var release = gamePlatform.Releases[0];
                if (release.Companies.Find(
                        c => c.Role.StartsWith("published", StringComparison.InvariantCultureIgnoreCase)
                    )
                    is { } company)
                {
                    var publisher = editGameViewModel.Publishers.FirstOrDefault(p => p.Name == company.Name);

                    if (publisher == null)
                    {
                        var mobyGamesUrl = await GetCanonicalMobyGamesCompanyUrl(company);

                        publisher = new Publisher(company.Name, company.Id)
                        {
                            Links = new List<string> { mobyGamesUrl.ToString() }
                        };

                        editGameViewModel.Publishers.Add(publisher);
                    }

                    editGameViewModel.GamePublisher = publisher;
                }

                var developerCollection = editGameViewModel.Developers.ToList();

                foreach (var devEntry in release.Companies.Where(
                             c => c.Role.StartsWith("developed", StringComparison.InvariantCultureIgnoreCase)
                         ))
                {
                    var developer = developerCollection.Find(d => d.Name == devEntry.Name);

                    if (developer == null)
                    {
                        var mobyGamesUrl = await GetCanonicalMobyGamesCompanyUrl(devEntry);

                        developer = new Developer(devEntry.Name, devEntry.Id)
                        {
                            Links = new List<string> { mobyGamesUrl.ToString() }
                        };

                        editGameViewModel.Developers.Add(developer);
                    }

                    editGameViewModel.GameDevelopers.Add(developer);
                }

                await GetScreenshots(client, gameEntry, preferredPlatform);
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
        }

        private void GetSpecs(Game gameEntry)
        {
            var platforms = Enum
                .GetValues(typeof(Platform))
                .Cast<Platform>()
                .Where(
                    platform => gameEntry.Platforms.Exists(
                        gamePlatform => gamePlatform.Name.Contains(platform.GetDescription())
                    )
                )
                .ToList();

            foreach (var platform in platforms)
            {
                editGameViewModel.GamePlatforms.Add(platform);
            }
        }

        private async Task GetScreenshots(MobyGamesClient client, Game gameEntry, GamePlatform gamePlatform)
        {
            var screenshotEntries = await client.GameScreenshots(gameEntry.Id, gamePlatform.Id);

            var screenshotLimit = AppSettingsHelper.Current.MobyGamesScreenshotLimit();

            var listItems = screenshotEntries.Take((int)screenshotLimit).ToList();

            var images = listItems
                .Select(item => new ScreenshotViewModel(item.ThumbnailImage, item.Image));

            foreach (var image in images)
            {
                editGameViewModel.GameScreenshots.Add(image);
            }
        }

        private Task<Uri> GetCanonicalMobyGamesCompanyUrl(GamePlatformReleaseCompany company) =>
            GetCanonicalMobyGamesUrl($"company/{company.Id}");

        private async Task<Uri> GetCanonicalMobyGamesUrl(string path)
        {
            using var httpClient = new HttpClient(
                new HttpClientHandler
                {
                    AllowAutoRedirect = false
                }
            );

            var url = new Uri(mobyGamesBaseUri, path);

            using var result = await httpClient.GetAsync(url);

            if (result.StatusCode is
                HttpStatusCode.Moved or
                HttpStatusCode.MovedPermanently or
                HttpStatusCode.PermanentRedirect)
            {
                return result.Headers.Location ?? throw new InvalidOperationException("Location header missing");
            }

            result.EnsureSuccessStatusCode();

            return url;
        }
    }
}
