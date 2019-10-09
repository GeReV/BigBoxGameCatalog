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
//        private GameCopy game = new GameCopy();
//
//        public AddGameDialog()
//        {
//            InitializeComponent();
//        }
//
//        protected override void OnShown(EventArgs e)
//        {
//            DataContext = game;
//
//            okButton.Command = new Command(async (sender, _) => Close(await BuildGame()));
//            AbortButton.Command = new Command((sender, _) => Close());
//        }
//
//        private async Task<GameCopy> BuildGame()
//        {
//            gameInfoForm.StatusLabel.Text = "Saving...";
//            gameInfoForm.ProgressBar.Visible = true;
//
//            game.Developers.Clear();
//
//            var developers = CatalogApplication.Instance.Database
//                .GetDevelopersCollection()
//                .FindAll()
//                .ToList();
//
//            foreach (var slug in gameInfoForm.DeveloperList.SelectedKeys)
//            {
//                var dev = developers.First(d => d.Slug == slug);
//
//                game.Developers.Add(dev);
//            }
//
//            BuildItems();
//
//            game.Platform = (Platform)gameInfoForm.PlatformList
//                    .SelectedValues
//                    .Aggregate(0, (aggregate, platform) => aggregate | (int) platform);
//
//            gameInfoForm.StatusLabel.Text = "Downloading screenshots...";
//
//            game.Screenshots = new ObservableCollection<Image>(await DownloadScreenshots(game.MobyGamesSlug));
//
//            game.Notes = gameInfoForm.NotesTextArea.Text;
//
//            return game;
//        }
//
//        private void BuildItems()
//        {
//            if (gameInfoForm.HasBoxCheckbox.Checked.GetValueOrDefault())
//            {
//                game.Items.Add(ItemTypes.BigBox.CreateItem());
//            }
//
//            foreach (var medium in BuildGameMedia())
//            {
//                game.Items.Add(medium);
//            }
//        }
//
//        private List<Item> BuildGameMedia()
//        {
//            var media = new List<Item>();
//
//            foreach (var pair in gameInfoForm.AddMediaPanel.MediaValues)
//            {
//                if (pair.Value <= 0)
//                {
//                    continue;
//                }
//
//                var batch = Enumerable
//                    .Range(0, pair.Value)
//                    .Select(_ => pair.Key.CreateItem());
//
//                media.AddRange(batch);
//            }
//
//            return media;
//        }
//
//        private async Task<Image[]> DownloadScreenshots(string gameSlug)
//        {
//            var downloadUrls = gameInfoForm.Screenshots
//                .SelectedValues
//                .Cast<ImageListItem>()
//                .Select(item => (string) item.Tag);
//
//
//            var totalProgress = new AggregateProgress<int>(progressValues =>
//                {
//                    gameInfoForm.ProgressBar.Value = (int) progressValues.Average();
//                });
//
//            var scraper = new Scraper();
//
//            var downloadTasks = new List<Task<ImageEntry>>();
//
//            foreach (var downloadUrl in downloadUrls)
//            {
//                var progress = new Progress<int>();
//
//                totalProgress.Add(progress);
//                downloadTasks.Add(Task.Run(() => scraper.DownloadScreenshot(downloadUrl, progress)));
//            }
//
//            var screenshotDirectory = Path.Combine(CatalogApplication.Instance.HomeDirectory, "screenshots", gameSlug);
//
//            if (!Directory.Exists(screenshotDirectory))
//            {
//                Directory.CreateDirectory(screenshotDirectory);
//            }
//
//            var saveImageTasks = downloadTasks
//                .Select(task => task.ContinueWith(t =>
//                {
//                    var filename = t.Result.Url.Segments.Last();
//
//                    var destination = Path.Combine(screenshotDirectory, filename);
//
//                    File.WriteAllBytes(destination, t.Result.Data);
//
//                    return new Image
//                    {
//                        RelativePath = destination.Substring(CatalogApplication.Instance.HomeDirectory.Length)
//                    };
//                }));
//
//            return await Task.WhenAll(saveImageTasks);
//        }
    }
}