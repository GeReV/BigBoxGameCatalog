using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using File = System.IO.File;

namespace Catalog
{
    public class ImageDownloader
    {
        private readonly HttpClient httpClient;

        public ImageDownloader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string[]> DownloadImages(
            string screenshotDirectory,
            IEnumerable<Uri> downloadUrls,
            IProgress<int>? progress = null
        )
        {
            var totalProgress =
                new AggregateProgress<int>(progressValues => { progress?.Report((int)progressValues.Average()); });

            var downloadTasks = new List<Task<HttpResponseMessage>>();

            foreach (var downloadUrl in downloadUrls)
            {
                var itemProgress = new Progress<int>();

                totalProgress.Add(itemProgress);
                downloadTasks.Add(httpClient.GetAsync(downloadUrl));
            }

            EnsureDirectory(screenshotDirectory);

            var saveImageTasks = downloadTasks
                .Select(
                    async task =>
                    {
                        var response = await task;
                        return await WriteImageAsync(screenshotDirectory, response);
                    }
                )
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

        private static void EnsureDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static async Task<string> WriteImageAsync(
            string directory,
            HttpResponseMessage response,
            string? filename = null
        )
        {
            filename ??= response.RequestMessage?.RequestUri?.Segments.Last();
            filename ??= $"{DateTime.Now.ToString("yyyyMMddhhmmss")}.jpg";

            var destination = Path.Combine(directory, filename);

            await using var stream = File.Create(destination);

            await response.Content.CopyToAsync(stream);

            return destination;
        }
    }
}
