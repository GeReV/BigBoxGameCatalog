using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Model;
using Catalog.Scrapers.MobyGames.Model;
using File = System.IO.File;

namespace Catalog.Scrapers.MobyGames
{
    public class ImageDownloader
    {
        private readonly IWebClient webClient;
        private readonly Scraper scraper;

        public ImageDownloader(IWebClient webClient)
        {
            this.webClient = webClient;

            scraper = new Scraper(webClient);
        }

        public async Task<string[]> DownloadScreenshots(string screenshotDirectory, IEnumerable<string> downloadUrls,
            IProgress<int> progress = null)
        {
            var totalProgress = new AggregateProgress<int>(progressValues =>
            {
                progress?.Report((int) progressValues.Average());
            });

            var downloadTasks = new List<Task<ImageEntry>>();

            foreach (var downloadUrl in downloadUrls)
            {
                var itemProgress = new Progress<int>();

                totalProgress.Add(itemProgress);
                downloadTasks.Add(Task.Run(() => scraper.DownloadScreenshot(downloadUrl, itemProgress)));
            }

            EnsureDirectory(screenshotDirectory);

            var saveImageTasks = downloadTasks
                .Select(task => task.ContinueWith(t => WriteImage(screenshotDirectory, t)))
                .ToList();

            try
            {
                return await Task.WhenAll(saveImageTasks);
            }
            catch (AggregateException)
            {
                return saveImageTasks
                    .Where(t => t.Status == TaskStatus.RanToCompletion)
                    .Select(t => t.Result)
                    .ToArray();
            }
        }

        public Task<string> DownloadCoverArt(string directory, string url, IProgress<int> progress = null)
        {
            EnsureDirectory(directory);

            return Task.Run(() => scraper.DownloadCoverArt(url, progress))
                .ContinueWith(t => WriteImage(directory, t, "cover.jpg"));
        }

        private static void EnsureDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string WriteImage(string directory, Task<ImageEntry> t, string filename = null)
        {
            filename ??= t.Result.Url.Segments.Last();

            var destination = Path.Combine(directory, filename);

            File.WriteAllBytes(destination, t.Result.Data);

            return destination;
        }
    }
}
