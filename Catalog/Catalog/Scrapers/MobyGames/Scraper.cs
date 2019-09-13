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
        public ScraperException()
        {
        }

        public ScraperException(string message) : base(message)
        {
        }

        public ScraperException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ScraperException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }

    public class Scraper
    {

        public Scraper()
        {
        }

        const string SEARCH_RESULT = "searchResult";
        const string SEARCH_TITLE = "searchTitle";
        const string SEARCH_DETAILS = "searchDetails";
            
        const string CORE_GAME_RELEASE_ID = "coreGameRelease";
        const string GAME_NAME_TITLE = "niceHeaderTitle";

        const string OFFICIAL_SCREENSHOTS_ID = "official_screenshots";
        const string THUMBNAIL_GALLERY = "thumbnailGallery";

        public List<SearchResult> Search(string term)
        {
            var doc = LoadUrl(BuildSearchUrl(term));

            var results = doc.DocumentNode.SelectNodesByClass(SEARCH_RESULT, "div");


            var entries = results
                .Where(result =>
                    result.SelectSingleNodeByClass(SEARCH_TITLE)?.PlainInnerText().StartsWith("Game:") ?? false)
                .Select(result =>
                {
                    var entryLink = result.SelectSingleNodeByClass(SEARCH_TITLE).SelectSingleNode("a");

                    var searchResult = ExtractNamedEntryFromNode<SearchResult>(entryLink);

                    searchResult.Releases = result.SelectSingleNodeByClass(SEARCH_DETAILS).SelectNodes("span")
                        .Select(sp => sp.PlainInnerText()).ToArray();

                    return searchResult;
                })
                .ToList();

            return entries;
        }

        public GameEntry GetGame(string slug)
        {
            var url = BuildGameUrl(slug);

            var doc = LoadUrl(url).DocumentNode;

            var details = doc.SelectSingleNodeById(CORE_GAME_RELEASE_ID);


            return new GameEntry
            {
                Name = doc.SelectSingleNodeByClass(GAME_NAME_TITLE).SelectSingleNode("a").PlainInnerText(),
                Slug = slug,
                Url = url,
                Publisher = ExtractPublisher(details),
                Developers = ExtractDevelopers(details),
                Platforms = ExtractPlatforms(details),
                ReleaseDate = ExtractReleaseDate(details),
            };
        }

        public ScreenshotEntry[] GetGameScreenshots(string slug)
        {
            var url = $"{BuildGameUrl(slug)}/screenshots";

            var doc = LoadUrl(url).DocumentNode;

            var officialScreenshots = doc
                .SelectSingleNodeById(OFFICIAL_SCREENSHOTS_ID)
                ?.SelectSingleNode($"./following-sibling::*[@class='{THUMBNAIL_GALLERY}']");

            return ExtractScreenshots(new Uri(url), officialScreenshots).ToArray();
        }

        private IEnumerable<ScreenshotEntry> ExtractScreenshots(Uri baseUri, HtmlNode gallery)
        {
            if (gallery == null)
            {
                return new List<ScreenshotEntry>();
            }
            
            return gallery
                .SelectNodes(".//a")
                .Select(thumbnail => new ScreenshotEntry
                {
                    Url = thumbnail.GetAttributeValue("href", null),
                    Thumbnail = new Uri(baseUri,thumbnail.SelectSingleNode("img").GetAttributeValue("src", null)).ToString()
                });
        }

        private static string BuildGameUrl(string slug)
        {
            return $"https://www.mobygames.com/game/{Uri.EscapeUriString(slug)}";
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
            return details.SelectSingleNode($".//*[.='{title}']")?.NextSibling;
        }

        private static HtmlNode SelectNodeFollowingTitleStartingWith(HtmlNode details, string title)
        {
            return details.SelectSingleNode($".//*[starts-with(text(),'{title}')]")?.NextSibling;
        }

        private static PublisherEntry ExtractPublisher(HtmlNode details)
        {
            var publisherNode = SelectNodeFollowingTitle(details, "Published by")?.SelectSingleNode("a");

            if (publisherNode == null)
            {
                return null;
            }

            return ExtractNamedEntryFromNode<PublisherEntry>(publisherNode);
        }


        private static DeveloperEntry[] ExtractDevelopers(HtmlNode details)
        {
            var developerNodes = SelectNodeFollowingTitle(details, "Developed by")?.SelectNodes("a");

            if (developerNodes == null)
            {
                return new DeveloperEntry[0];
            }

            return developerNodes.Select(ExtractNamedEntryFromNode<DeveloperEntry>).ToArray();
        }

        private static string[] ExtractPlatforms(HtmlNode details)
        {
            return SelectNodeFollowingTitleStartingWith(details, "Platform")
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