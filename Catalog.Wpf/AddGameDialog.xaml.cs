using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Forms.Controls;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Catalog.Wpf.ViewModel;
using Eto.Drawing;
using Eto.Forms;
using Xceed.Wpf.Toolkit;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Window = System.Windows.Window;

namespace Catalog.Wpf
{
    public partial class AddGameDialog : Window
    {
        public AddGameDialog()
        {
            InitializeComponent();

            var database = Application.Current.Database();

            var publishers = database.GetPublishersCollection().FindAll();
            var developers = database.GetDevelopersCollection().FindAll();

            ViewModel = new AddGameViewModel(publishers, developers);
        }

        public AddGameViewModel ViewModel
        {
            get => (AddGameViewModel) DataContext;
            set => DataContext = value;
        }

        private void TitleTextbox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TitleTextbox.Text) && e.Key == Key.Enter)
            {
                SearchMobyGames(TitleTextbox.Text.Trim());
            }
        }

        private async void SearchMobyGames(string term)
        {
            if (!SearchMobyGamesButton.IsEnabled)
            {
                return;
            }

            SearchMobyGamesButton.IsEnabled = false;

            var scraper = new Scraper();

            var entries = await Task.Run(() => scraper.Search(term));

            var disambiguationDialog = new GameDisambiguationDialog(entries);

            if (disambiguationDialog.ShowDialog() != true)
            {
                return;
            }

            var gameEntry = scraper.GetGame(disambiguationDialog.SelectedResult.Slug);

            GetSpecs(gameEntry);

            GetScreenshots(gameEntry);

            ViewModel.GameScreenshots.Clear();

            ViewModel.GameTitle = gameEntry.Name;
            ViewModel.GameMobyGamesSlug = gameEntry.Slug;
            ViewModel.GameLinks.Add(gameEntry.Url);

            var publisher = ViewModel.Publishers.ToList().Find(p => p.Slug == gameEntry.Publisher.Slug);

            if (publisher == null)
            {
                publisher = new Publisher
                {
                    Name = gameEntry.Publisher.Name,
                    Slug = gameEntry.Publisher.Slug,
                    Links = new[] {gameEntry.Publisher.Url}
                };

                ViewModel.Publishers.Add(publisher);
            }

            ViewModel.GamePublisher = publisher;

            var developerCollection = ViewModel.Developers.ToList();

            foreach (var devEntry in gameEntry.Developers)
            {
                var developer = developerCollection.Find(d => d.Slug == devEntry.Slug);

                if (developer == null)
                {
                    developer = new Developer
                    {
                        Name = devEntry.Name,
                        Slug = devEntry.Slug,
                        Links = new[] {devEntry.Url},
                    };

                    ViewModel.Developers.Add(developer);
                }

                ViewModel.GameDevelopers.Add(developer);
            }

            SearchMobyGamesButton.IsEnabled = true;
        }

        private async void GetSpecs(GameEntry gameEntry)
        {
            var specs = await Task.Run(() => new Scraper().GetGameSpecs(gameEntry.Slug));

            PlatformCheckList.SelectedItemsOverride = Enum
                .GetValues(typeof(Platform))
                .Cast<Platform>()
                .Where(platform => specs.Platforms.Contains(platform.GetDescription()))
                .ToList();
        }

        private async void GetScreenshots(GameEntry gameEntry)
        {
            IEnumerable<ScreenshotEntry> screenshotEntries = await Task.Run(() =>
                new Scraper().GetGameScreenshots(gameEntry.Slug)
            );

            var listItems = screenshotEntries.Take(20).ToList();

            var images = listItems.Select(item => new ScreenshotViewModel
            {
                Url = item.Url,
                ThumbnailUrl = item.Thumbnail,
                ThumbnailSource = new BitmapImage(new Uri(item.Thumbnail))
            });

            foreach (var image in images)
            {
                ViewModel.GameScreenshots.Add(image);
            }

            Screenshots.SelectAll();
        }

        private void AddGameDialog_OnContentRendered(object sender, EventArgs e)
        {
            TitleTextbox.Focus();
        }

        private async void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Status = AddGameViewModel.ViewStatus.DownloadingScreenshots;

            var game = await BuildGame();

            ViewModel.Status = AddGameViewModel.ViewStatus.Idle;

//            DialogResult = true;

            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
//            DialogResult = false;

            Close();
        }

        private async Task<GameCopy> BuildGame()
        {
            var screenshotDirectory = Path.Combine(Application.Current.HomeDirectory(), "screenshots",
                ViewModel.GameMobyGamesSlug);

            var screenshots = await ScreenshotDownloader.DownloadScreenshots(
                screenshotDirectory,
                Screenshots.SelectedItems
                    .Cast<ScreenshotViewModel>()
                    .Select(ss => ss.Url),
                new Progress<int>(percentage => ViewModel.SaveProgress = percentage)
            );

            return new GameCopy
            {
                Title = ViewModel.GameTitle,
                MobyGamesSlug = ViewModel.GameMobyGamesSlug,
                Platform = ViewModel.GamePlatform,
                Publisher = ViewModel.GamePublisher,
                Developers = ViewModel.GameDevelopers.ToList(),
                Items = ViewModel.GameItems.Select(item => item.BuildItem()),
                Links = ViewModel.GameLinks.ToList(),
                Notes = ViewModel.GameNotes,
                TwoLetterIsoLanguageName = ViewModel.GameTwoLetterIsoLanguageName,
                ReleaseDate = ViewModel.GameReleaseDate,
                Screenshots = screenshots
            };
        }

        private void AddItemMenu_OnClick(object sender, RoutedEventArgs e)
        {
            AddItemButton.IsOpen = false;
        }
    }
}