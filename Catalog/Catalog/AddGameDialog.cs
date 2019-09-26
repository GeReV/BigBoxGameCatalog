using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Catalog.Forms;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Eto.Drawing;
using Eto.Forms;
using File = System.IO.File;
using Image = Catalog.Model.Image;
using Platform = Catalog.Model.Platform;

namespace Catalog
{
    public partial class AddGameDialog : Dialog<GameCopy>
    {
        private GameCopy game = new GameCopy();

        private readonly ObservableCollection<Publisher> publishers = new ObservableCollection<Publisher>(
            CatalogApplication.Instance.Database.GetPublishersCollection().FindAll()
        );

        private readonly ObservableCollection<Developer> developers = new ObservableCollection<Developer>(
            CatalogApplication.Instance.Database.GetDevelopersCollection().FindAll()
        );

        public AddGameDialog()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            DefaultControl.Focus();

            DataContext = game;

            okButton.Command = new Command(async (sender, _) => Close(await BuildGame()));
            AbortButton.Command = new Command((sender, _) => Close());

            titleTextbox.KeyUp += TitleTextbox_KeyUp;
            titleTextbox.BindDataContext<TextBox, GameCopy, string>(c => c.Text, g => g.Title);

            searchMobyGamesButton.Command = new Command((sender, _) => SearchMobyGames(titleTextbox.Text.Trim()));

            publisherList.BindDataContext<ComboBox, GameCopy, object>(
                c => c.SelectedValue,
                g => g.Publisher
            );

            publisherList.DataStore = publishers;

            developerList.ItemTextBinding = Binding.Property<Developer, string>(d => d.Name);
            developerList.ItemKeyBinding = Binding.Property<Developer, string>(d => d.Slug);
            developerList.DataStore = developers;
            developerList.SelectedKeysBinding.BindDataContext<GameCopy>(
                (gc) => gc?.Developers.Select(d => d.Slug) ?? new List<string>(),
                null
            );
        }

        private async Task<GameCopy> BuildGame()
        {
            statusLabel.Text = "Saving...";
            progressBar.Visible = true;

            game.Developers.Clear();

            foreach (var slug in developerList.SelectedKeys)
            {
                var dev = developers.First(d => d.Slug == slug);

                game.Developers.Add(dev);
            }

            if (hasBoxCheckbox.Checked.GetValueOrDefault())
            {
                game.Items.Add(ItemTypes.BigBox.CreateItem());
            }

            game.Items.AddRange(BuildGameMedia());

            game.Platform = (Platform)platformList
                    .SelectedValues
                    .Aggregate(0, (aggregate, platform) => aggregate | (int) platform);

            statusLabel.Text = "Downloading screenshots...";

            game.Screenshots = (await DownloadScreenshots(game.MobyGamesSlug)).ToList();

            game.Notes = notes.Text;

            return game;
        }

        private List<Item> BuildGameMedia()
        {
            var media = new List<Item>();

            foreach (var pair in addMediaPanel.MediaValues)
            {
                if (pair.Value <= 0)
                {
                    continue;
                }

                var batch = Enumerable
                    .Range(0, pair.Value)
                    .Select(_ => pair.Key.CreateItem());

                media.AddRange(batch);
            }

            return media;
        }

        private async Task<Image[]> DownloadScreenshots(string gameSlug)
        {
            var downloadUrls = screenshots
                .SelectedValues
                .Cast<ImageListItem>()
                .Select(item => (string) item.Tag);


            var totalProgress = new AggregateProgress<int>(progressValues =>
                {
                    progressBar.Value = (int) progressValues.Average();
                });

            var scraper = new Scraper();

            var downloadTasks = new List<Task<ImageEntry>>();

            foreach (var downloadUrl in downloadUrls)
            {
                var progress = new Progress<int>();

                totalProgress.Add(progress);
                downloadTasks.Add(Task.Run(() => scraper.DownloadScreenshot(downloadUrl, progress)));
            }

            var screenshotDirectory = Path.Combine(CatalogApplication.Instance.HomeDirectory, "screenshots", gameSlug);

            if (!Directory.Exists(screenshotDirectory))
            {
                Directory.CreateDirectory(screenshotDirectory);
            }

            var saveImageTasks = downloadTasks
                .Select(task => task.ContinueWith(t =>
                {
                    var filename = t.Result.Url.Segments.Last();

                    var destination = Path.Combine(screenshotDirectory, filename);

                    File.WriteAllBytes(destination, t.Result.Data);

                    return new Image
                    {
                        RelativePath = destination.Substring(CatalogApplication.Instance.HomeDirectory.Length)
                    };
                }));

            return await Task.WhenAll(saveImageTasks);
        }

        private void TitleTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(titleTextbox.Text) && e.Key == Keys.Enter)
            {
                SearchMobyGames(titleTextbox.Text.Trim());
            }
        }

        private async void SearchMobyGames(string term)
        {
            searchMobyGamesButton.Enabled = false;

            var scraper = new Scraper();

            var entries = await Task.Run(() => scraper.Search(term));

            var choice = new GameDisambiguationDialog(entries).ShowModal();

            if (choice == null)
            {
                return;
            }

            var gameEntry = scraper.GetGame(choice.Slug);

            GetSpecs(gameEntry);

            GetScreenshots(gameEntry);

            game = new GameCopy
            {
                Title = gameEntry.Name,
                MobyGamesSlug = gameEntry.Slug,
                Links = new List<string> {gameEntry.Url},
            };

            var publisher = publishers.ToList().Find(p => p.Slug == gameEntry.Publisher.Slug);

            if (publisher == null)
            {
                publisher = new Publisher
                {
                    Name = gameEntry.Publisher.Name,
                    Slug = gameEntry.Publisher.Slug,
                    Links = new[] {gameEntry.Publisher.Url}
                };

                publishers.Add(publisher);
            }

            game.Publisher = publisher;

            var developerCollection = developers.ToList();

            var gameDevelopers = new List<Developer>();

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

                    developers.Add(developer);
                }

                gameDevelopers.Add(developer);
            }

            game.Developers = gameDevelopers;

            DataContext = game;
        }

        private async void GetSpecs(GameEntry gameEntry)
        {
            var specs = await Task.Run(() => new Scraper().GetGameSpecs(gameEntry.Slug));

            platformList.SelectedValues = Enum
                .GetValues(typeof(Platform))
                .Cast<Platform>()
                .Where(platform => specs.Platforms.Contains(platform.GetDescription()));

            var mediaTypesList = specs.MediaTypes.ToList();

            if (mediaTypesList.Exists(mt => mt.Contains("5.25\" Floppy")))
            {
                addMediaPanel.SetStepperValue(ItemTypes.Floppy525, 1);
            }
            else if (mediaTypesList.Exists(mt => mt.Contains("3.5\" Floppy")))
            {
                addMediaPanel.SetStepperValue(ItemTypes.Floppy35, 1);
            }
            else if (mediaTypesList.Exists(mt => mt.Contains("CD-ROM")))
            {
                addMediaPanel.SetStepperValue(ItemTypes.CdRom, 1);
            }
            else if (mediaTypesList.Exists(mt => mt.Contains("DVD-ROM")))
            {
                addMediaPanel.SetStepperValue(ItemTypes.DvdRom, 1);
            }
        }

        private async void GetScreenshots(GameEntry gameEntry)
        {
            IEnumerable<ScreenshotEntry> screenshotEntries = await Task.Run(() =>
                new Scraper().GetGameScreenshots(gameEntry.Slug)
            );

            var listItems = screenshotEntries.Take(20)
                .Select(ss => new ImageListItem
                {
                    Key = ss.Thumbnail,
                    Tag = ss.Url,
                })
                .ToList();

            var images = new ObservableCollection<ImageListItem>(listItems);

            screenshots.DataStore = images;
            screenshots.SelectAll();

            var imageLoadTasks = listItems
                .Select(item => Task.Run(async () => new Tuple<ImageListItem, Bitmap>(
                    item,
                    new Bitmap(await new WebClient().DownloadDataTaskAsync(item.Key))
                )))
                .ToList();

            while (imageLoadTasks.Count > 0)
            {
                var task = await Task.WhenAny(imageLoadTasks);

                imageLoadTasks.Remove(task);

                var (item, image) = task.Result;

                item.Image = image;

                var index = images.IndexOf(item);

                images[index] = item;

                screenshots.Select(index);
            }

            searchMobyGamesButton.Enabled = true;
        }
    }
}