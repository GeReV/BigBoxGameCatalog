using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Catalog.Scrapers.MobyGames.Model;
using HtmlAgilityPack;

namespace Catalog.Scrapers.MobyGames
{

    [Serializable]
    public class ScraperException : Exception
    {
        public ScraperException() { }
        public ScraperException(string message) : base(message) { }
        public ScraperException(string message, Exception inner) : base(message, inner) { }
        protected ScraperException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class Scraper
    {
        public Scraper()
        {
        }

        public List<SearchResult> Search(string term)
        {
            var doc = LoadUrl(BuildSearchUrl(term));

            var results = doc.DocumentNode.SelectNodesByClass("searchResult", "div");

            var entries = results
                .Where(result => result.SelectSingleNodeByClass("searchTitle")?.PlainInnerText().StartsWith("Game:") ?? false)
                .Select(result =>
                {
                    var entryLink = result.SelectSingleNodeByClass("searchTitle").SelectSingleNode("a");

                    var searchResult = ExtractNamedEntryFromNode<SearchResult>(entryLink);

                    searchResult.Releases = result.SelectSingleNodeByClass("searchDetails").SelectNodes("span").Select(sp => sp.PlainInnerText()).ToArray();

                    return searchResult;
                })
                .ToList();

            return entries;
        }

        public GameEntry GetGame(string slug)
        {
            var url = BuildGameUrl(slug);

            var doc = LoadUrl(url).DocumentNode;

            var details = doc.SelectSingleNodeById("coreGameRelease");


            return new GameEntry
            {
                Name = doc.SelectSingleNodeByClass("niceHeaderTitle").SelectSingleNode("a").PlainInnerText(),
                Slug = slug,
                Url = url,
                Publisher = ExtractPublisher(details),
                Developers = ExtractDevelopers(details),
                Platforms = ExtractPlatforms(details),
                ReleaseDate = ExtractReleaseDate(details),
            };
        }

        private static string BuildGameUrl(string slug)
        {
            return string.Format("https://www.mobygames.com/game/{0}", Uri.EscapeUriString(slug));
        }

        private static string BuildSearchUrl(string term)
        {
            var url = new UriBuilder("https://www.mobygames.com/search/quick");

            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("q", term);

            url.Query = queryString.ToString();

            return url.ToString();
        }

        private HtmlDocument LoadUrl(string url)
        {
            var web = new HtmlWeb();
            return web.Load(url);
        }

        private static string ExtractSlug(string href)
        {
            return new Uri(href).Segments.Last();
        }

        private static T ExtractNamedEntryFromNode<T>(HtmlNode node) where T : NamedEntry, new()
        {
            if (node == null || node.Name != "a")
            {
                throw new ScraperException("Attempt to extract named entry from node failed.");
            }

            var url = node.GetAttributeValue("href", string.Empty);

            return new T
            {
                Name = node.PlainInnerText(),
                Url = url,
                Slug = ExtractSlug(url),
            };
        }

        private static HtmlNode SelectNodeFollowingTitle(HtmlNode details, string title)
        {
            return details.SelectSingleNode(string.Format(".//*[.='{0}']", title)).NextSibling;
        }

        private static PublisherEntry ExtractPublisher(HtmlNode details)
        {
            var publisherNode = SelectNodeFollowingTitle(details, "Published by").SelectSingleNode("a");

            return ExtractNamedEntryFromNode<PublisherEntry>(publisherNode);
        }


        private static DeveloperEntry[] ExtractDevelopers(HtmlNode details)
        {
            var developerNodes = SelectNodeFollowingTitle(details, "Developed by").SelectNodes("a");

            return developerNodes.Select(ExtractNamedEntryFromNode<DeveloperEntry>).ToArray();
        }

        private static string[] ExtractPlatforms(HtmlNode details)
        {
            return SelectNodeFollowingTitle(details, "Platforms")
                .SelectNodes("a")
                .Select(node => node.PlainInnerText())
                .ToArray();
        }

        private static string ExtractReleaseDate(HtmlNode details)
        {
            return SelectNodeFollowingTitle(details, "Released")
                .SelectSingleNode("a")
                .PlainInnerText();
        }
    }
}
