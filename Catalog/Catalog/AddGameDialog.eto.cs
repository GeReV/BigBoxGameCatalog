using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Forms;
using Catalog.Forms.Controls;
using Eto.Forms;
using Eto.Drawing;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Image = Eto.Drawing.Image;

namespace Catalog
{
    partial class AddGameDialog : Dialog<GameCopy>
    {
        private GameCopy game = new GameCopy();

        protected TextBox titleTextbox;
        protected ComboBox publisherList;
        protected CheckBoxList developerList;
        protected CheckBox hasBoxCheckbox;
        protected ThumbnailSelect screenshots;

        private ObservableCollection<Publisher> publishers = new ObservableCollection<Publisher>(
            CatalogApplication.Instance.Database.GetPublishersCollection().FindAll()
        );

        private ObservableCollection<Developer> developers = new ObservableCollection<Developer>(
            CatalogApplication.Instance.Database.GetDevelopersCollection().FindAll()
        );

        protected Control DefaultControl { get; private set; }

        void InitializeComponent()
        {
            Title = "Add Game Dialog";
            ClientSize = new Size(800, 600);
            Padding = 10;

            DataContext = game;

            PositiveButtons.Add(new Button
            {
                Text = "OK",
                Command = new Command((sender, e) => Close(game))
            });
            NegativeButtons.Add(new Button
            {
                Text = "Cancel",
                Command = new Command((sender, e) => Close())
            });

            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(5, 5),
            };
            
            Content = layout;

            layout.BeginVertical();

            titleTextbox = new TextBox();
            titleTextbox.KeyUp += TitleTextbox_KeyUp;
            titleTextbox.BindDataContext<TextBox, GameCopy, string>(c => c.Text, g => g.Title);

            DefaultControl = titleTextbox;

            AddRow(layout, "Title", l =>
            {
                l.Add(titleTextbox, true);
                layout.Add(new Button
                {
                    Text = "Search MobyGames",
                    Command = new Command((sender, e) => SearchMobyGames(titleTextbox.Text.Trim()))
                });
            });

            publisherList = new ComboBox
            {
                AutoComplete = true,
                DataStore = publishers,
            };

            publisherList.BindDataContext<ComboBox, GameCopy, object>(
                c => c.SelectedValue,
                g => g.Publisher
            );

            AddRow(layout, "Publisher", publisherList);

            developerList = new CheckBoxList()
            {
                Orientation = Orientation.Vertical,
                ItemTextBinding = Binding.Property<Developer, string>(d => d.Name),
                ItemKeyBinding = Binding.Property<Developer, string>(d => d.Slug),
                DataStore = developers,
            };

            AddRow(layout, "Developers", new Scrollable
            {
                BackgroundColor = Colors.White,
                ExpandContentHeight = false,
                Height = 120,
                Content = developerList,
            });
            
            developerList.SelectedKeysBinding.BindDataContext<GameCopy>(
                (gc) => gc?.Developers.Select(d => d.Slug) ?? new List<string>(),
                (gc, slugs) =>
                {
                    gc.Developers.Clear();
                    
                    foreach (var slug in slugs)
                    {
                        var dev = developers.First(d => d.Slug == slug);

                        gc.Developers.Add(dev);
                    }
                }
            );

            hasBoxCheckbox = new CheckBox();
            hasBoxCheckbox.CheckedBinding.BindDataContext<GameCopy>(
                g => g?.GameBox != null,
                (g, b) => g.GameBox = b.GetValueOrDefault(true) ? new GameBox() : null
            );

            AddRow(layout, "Has Game Box", hasBoxCheckbox);

            screenshots = new ThumbnailSelect();

            AddRow(layout, "Screenshots", screenshots);

            layout.EndVertical();

            layout.BeginVertical();
            layout.AddSpace();
            layout.AddRow(
                null,
                AbortButton,
                DefaultButton
            );

            layout.EndVertical();
        }

        private void AddRow(DynamicLayout layout, string label, Control control)
        {
            AddRow(layout, label, l => l.Add(control, true));
        }

        private void AddRow(DynamicLayout layout, string label, Action<DynamicLayout> func)
        {
            layout.BeginHorizontal();
            layout.Add(new Label {Text = label, Width = 200});
            func(layout);
            layout.EndHorizontal();
        }

        private void TitleTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(titleTextbox.Text) && e.Key == Keys.Enter)
            {
                SearchMobyGames(titleTextbox.Text.Trim());
            }
        }

        private void SearchMobyGames(string term)
        {
            var scraper = new Scraper();

            var entries = scraper.Search(term);

            var choice = new GameDisambiguationDialog(entries).ShowModal();

            if (choice == null)
            {
                return;
            }

            var gameEntry = scraper.GetGame(choice.Slug);

            ShowScreenshots(gameEntry);

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
            
            // Can this be done via DataContext?
            developerList.UpdateBindings();
        }

        private async void ShowScreenshots(GameEntry gameEntry)
        {
            IEnumerable<ScreenshotEntry> screenshotEntries = new Scraper().GetGameScreenshots(gameEntry.Slug);

            var listItems = screenshotEntries.Take(5)
                .Select(async ss => new ImageListItem
                {
                    Key = ss.Thumbnail,
                    Tag = ss.Url,
                    Image = new Bitmap(await new WebClient().DownloadDataTaskAsync(ss.Thumbnail))
                })
                .ToList();

            var images = new ObservableCollection<ImageListItem>();

            screenshots.DataStore = images;

            while (listItems.Count > 0)
            {
                var task = await Task.WhenAny(listItems);

                listItems.Remove(task);

                images.Add(task.Result);
            }

            screenshots.SelectAll();
        }
    }
}