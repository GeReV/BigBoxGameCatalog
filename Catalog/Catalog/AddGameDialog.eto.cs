using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private GameCopy game;

        protected TextBox titleTextbox;
        protected ComboBox publisherList;
        protected GridView<Developer> developerList;
        protected CheckBox hasBoxCheckbox;
        protected StackLayout screenshots;

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

            developerList = new GridView<Developer>
            {
                AllowMultipleSelection = true,
                ShowHeader = false,
                DataStore = developers,
            };

            developerList.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Delegate<Developer, bool?>(
                        d => game.Developers.Contains(d),
                        (developer, b) =>
                        {
                            if (b.GetValueOrDefault(false))
                            {
                                game.Developers.Add(developer);
                            }
                            else
                            {
                                game.Developers.Remove(developer);
                            }
                        }
                    )
                },
                Editable = true,
            });
            developerList.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell {TextBinding = Binding.Property<Developer, string>(d => d.Name)}
            });

            AddRow(layout, "Developers", developerList);

            hasBoxCheckbox = new CheckBox();
            hasBoxCheckbox.CheckedBinding.BindDataContext<GameCopy>(
                g => g.GameBox != null,
                (g, b) => g.GameBox = b.GetValueOrDefault(true) ? new GameBox() : null
            );

            AddRow(layout, "Has Game Box", hasBoxCheckbox);

            screenshots = new StackLayout
            {
                BackgroundColor = Colors.White,
                Padding = new Padding(5),
                Spacing = 5,
                Orientation = Orientation.Horizontal,
                Height = 120 + 10 * 2
            };

            AddRow(layout, "Screenshots", new Scrollable
            {
                Content = screenshots,
            });

            layout.EndVertical();

            layout.BeginVertical();
            layout.AddSpace();
            layout.AddRow(
                null,
                AbortButton,
                DefaultButton
            );

            layout.EndVertical();

            Content = layout;
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

            var gameDevelopers = new HashSet<Developer>();

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

        private async void ShowScreenshots(GameEntry gameEntry)
        {
            IEnumerable<ScreenshotEntry> screenshotEntries = new Scraper().GetGameScreenshots(gameEntry.Slug);

            var requests = screenshotEntries.Take(5)
                .Select(async ss => new Bitmap(await new WebClient().DownloadDataTaskAsync(ss.Thumbnail)))
                .ToList();

            while (requests.Count > 0)
            {
                var task = await Task.WhenAny(requests);

                requests.Remove(task);
                
                screenshots.Items.Add(new Panel
                {
                    BackgroundColor = Colors.LightBlue,
                    Padding = new Padding(5),
                    Content = new ImageView
                    {
                        Image = task.Result,
                        Height = 120,
                    }
                });
            }
        }
    }
}