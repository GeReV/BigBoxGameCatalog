using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Catalog.Scrapers
{
    public class CachingWebClient : IWebClient
    {
        public HtmlDocument Load(string url)
        {
            var filename = UrlToFilename(url);
            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);

            if (File.Exists(filename))
            {
                var doc = new HtmlDocument();

                doc.Load(path);

                return doc;
            }

            var webDocument = new HtmlWeb().Load(url);

            webDocument.Save(path);

            return webDocument;
        }

        private static string UrlToFilename(string url) => new Regex("[^A-Za-z0-9]+", RegexOptions.Compiled).Replace(url, "-");
    }
}