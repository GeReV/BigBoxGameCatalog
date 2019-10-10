using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Model;
using Catalog.Scrapers.MobyGames.Model;
using Eto.Forms;
using File = System.IO.File;

namespace Catalog.Scrapers.MobyGames
{
    public static class ScreenshotDownloader
    {
        public static async Task<Image[]> DownloadScreenshots(string screenshotDirectory, IEnumerable<string> downloadUrls, IProgress<int> progress = null)
        {
            var totalProgress = new AggregateProgress<int>(progressValues =>
            {
                progress?.Report((int) progressValues.Average());
            });

            var scraper = new Scraper();

            var downloadTasks = new List<Task<ImageEntry>>();

            foreach (var downloadUrl in downloadUrls)
            {
                var itemProgress = new Progress<int>();

                totalProgress.Add(itemProgress);
                downloadTasks.Add(Task.Run(() => scraper.DownloadScreenshot(downloadUrl, itemProgress)));
            }

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
                        Path = destination
                    };
                }));

            return await Task.WhenAll(saveImageTasks);
        }
    }
}