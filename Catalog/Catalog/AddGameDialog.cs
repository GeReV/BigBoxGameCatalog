using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Eto.Drawing;
using Eto.Forms;

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

            DefaultButton.Command = new Command((sender, _) => Close(BuildGame()));
            AbortButton.Command = new Command((sender, _) => Close());

            TitleTextbox.KeyUp += TitleTextbox_KeyUp;
            TitleTextbox.BindDataContext<TextBox, GameCopy, string>(c => c.Text, g => g.Title);

            SearchMobyGamesButton.Command = new Command((sender, _) => SearchMobyGames(TitleTextbox.Text.Trim()));

            PublisherList.BindDataContext<ComboBox, GameCopy, object>(
                c => c.SelectedValue,
                g => g.Publisher
            );

            PublisherList.DataStore = publishers;

            DeveloperList.ItemTextBinding = Binding.Property<Developer, string>(d => d.Name);
            DeveloperList.ItemKeyBinding = Binding.Property<Developer, string>(d => d.Slug);
            DeveloperList.DataStore = developers;
            DeveloperList.SelectedKeysBinding.BindDataContext<GameCopy>(
                (gc) => gc?.Developers.Select(d => d.Slug) ?? new List<string>(),
                null
            );

            HasBoxCheckbox.CheckedBinding.BindDataContext<GameCopy>(
                g => g?.GameBox != null,
                null
            );
        }

        private GameCopy BuildGame()
        {
            game.Developers.Clear();

            foreach (var slug in DeveloperList.SelectedKeys)
            {
                var dev = developers.First(d => d.Slug == slug);

                game.Developers.Add(dev);
            }

            if (HasBoxCheckbox.Checked.GetValueOrDefault())
            {
                game.GameBox = new GameBox();
            }

            return game;
        }

        private void TitleTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TitleTextbox.Text) && e.Key == Keys.Enter)
            {
                SearchMobyGames(TitleTextbox.Text.Trim());
            }
        }

        private async void SearchMobyGames(string term)
        {
            SearchMobyGamesButton.Enabled = false;

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
                Links = new List<string> {gameEntry.Url},
                Screenshots = new List<string>(),
                GameBox = new GameBox(),
            };

            var publisherEntry = publishers.ToList().Find(p => p.Slug == gameEntry.Publisher.Slug);

            if (publisherEntry == null)
            {
                var publisher = new Publisher
                {
                    Name = gameEntry.Publisher.Name,
                    Slug = gameEntry.Publisher.Slug,
                    Links = new[] {gameEntry.Publisher.Url}
                };

                game.Publisher = publisher;

                publishers.Add(publisher);
            }

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

            PlatformList.SelectedValues = Enum
                .GetValues(typeof(Platform))
                .Cast<Platform>()
                .Where(platform => specs.Platforms.Contains(platform.GetDescription()));

            var mediaTypesList = specs.MediaTypes.ToList();

            if (mediaTypesList.Exists(mt => mt.Contains("5.25\" Floppy")))
            {
                AddMediaPanel.SetStepperValue(MediaType.Floppy525, 1);
            }
            else if (mediaTypesList.Exists(mt => mt.Contains("3.5\" Floppy")))
            {
                AddMediaPanel.SetStepperValue(MediaType.Floppy35, 1);
            }
            else if (mediaTypesList.Exists(mt => mt.Contains("CD-ROM")))
            {
                AddMediaPanel.SetStepperValue(MediaType.CdRom, 1);
            }
            else if (mediaTypesList.Exists(mt => mt.Contains("DVD-ROM")))
            {
                AddMediaPanel.SetStepperValue(MediaType.DvdRom, 1);
            }
        }

        private async void GetScreenshots(GameEntry gameEntry)
        {
            IEnumerable<ScreenshotEntry> screenshotEntries = new Scraper().GetGameScreenshots(gameEntry.Slug);

            var listItems = screenshotEntries.Take(20)
                .Select(ss => new ImageListItem
                {
                    Key = ss.Thumbnail,
                    Tag = ss.Url,
                })
                .ToList();

            var images = new ObservableCollection<ImageListItem>(listItems);

            Screenshots.DataStore = images;
            Screenshots.SelectAll();

            var imageLoadTasks = listItems
                .Select(async item => new Tuple<ImageListItem, Bitmap>(
                    item,
                    new Bitmap(await new WebClient().DownloadDataTaskAsync(item.Key))
                ))
                .ToList();

            while (imageLoadTasks.Count > 0)
            {
                var task = await Task.WhenAny(imageLoadTasks);

                imageLoadTasks.Remove(task);

                var (item, image) = task.Result;

                item.Image = image;
            }

            SearchMobyGamesButton.Enabled = true;
        }
    }
}