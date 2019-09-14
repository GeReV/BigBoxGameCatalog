using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

    public class Scraper : BaseScraper
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

        public Specs GetGameSpecs(string slug)
        {
            var url = $"{BuildGameUrl(slug)}/techinfo";

            var doc = LoadUrl(url).DocumentNode;

            var platformTechInfos = doc
                .SelectNodesByClass("techInfo")
                .ToDictionary(node => node.SelectSingleNode("thead").PlainInnerText().Trim());

            var platforms = new List<string>();

            if (platformTechInfos.ContainsKey("Windows"))
            {
                var minimumOs = SelectNodeWithText(
                        platformTechInfos["Windows"],
                        "Minimum OS Class Required"
                    )
                    ?.SelectFollowingNodeByTagName("td")
                    ?.PlainInnerText()
                    .Trim();

                platforms.Add(minimumOs);
            }

            if (platformTechInfos.ContainsKey("DOS"))
            {
                platforms.Add("DOS");
            }

            var mediaTypes = platformTechInfos
                .Where(pair => pair.Key == "DOS" || pair.Key == "Windows")
                .SelectMany(pair =>
                    SelectNodeWithText(pair.Value, "Media Type")
                        ?.SelectFollowingNodeByTagName("td")
                        ?.SelectNodes(".//a[not(img)]")
                )
                .Select(node => node.PlainInnerText())
                .Distinct();

            return new Specs
            {
                Platforms = platforms,
                MediaTypes = mediaTypes,
            };
        }

        public ScreenshotEntry[] GetGameScreenshots(string slug)
        {
            var url = $"{BuildGameUrl(slug)}/screenshots";

            var doc = LoadUrl(url).DocumentNode;

            var officialScreenshotsNode = doc
                .SelectSingleNodeById(OFFICIAL_SCREENSHOTS_ID)
                ?.SelectFollowingNodeByClass(THUMBNAIL_GALLERY);

            var baseUri = new Uri(url);

            var officialScreenshots = ExtractOfficialScreenshots(baseUri, officialScreenshotsNode).ToArray();

            var screenshots = ExtractScreenshots(baseUri, doc.SelectNodesByClass("thumbnail-image"));

            return officialScreenshots.Concat(screenshots).ToArray();
        }

        private IEnumerable<ScreenshotEntry> ExtractOfficialScreenshots(Uri baseUri, HtmlNode gallery)
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
                    Thumbnail = new Uri(baseUri, thumbnail.SelectSingleNode("img").GetAttributeValue("src", null))
                        .ToString()
                });
        }

        private readonly Regex BACKGROUND_IMAGE_REGEX = new Regex("background(?:-image)?:\\s*url\\('?(.*?)'?\\)");

        private IEnumerable<ScreenshotEntry> ExtractScreenshots(Uri baseUri, HtmlNodeCollection nodes)
        {
            if (nodes == null)
            {
                return new List<ScreenshotEntry>();
            }

            return nodes
                .Select(thumbnail =>
                {
                    var thumbnailPath = BACKGROUND_IMAGE_REGEX.Match(thumbnail.GetAttributeValue("style", ""))
                        ?.Groups[1].Value;

                    return new ScreenshotEntry
                    {
                        Url = thumbnail.GetAttributeValue("href", null),
                        Thumbnail = new Uri(baseUri, thumbnailPath)
                            .ToString()
                    };
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

        private static PublisherEntry ExtractPublisher(HtmlNode details)
        {
            var publisherNode = SelectNodeWithText(details, "Published by")
                ?.NextSibling
                .SelectSingleNode("a");

            if (publisherNode == null)
            {
                return null;
            }

            return ExtractNamedEntryFromNode<PublisherEntry>(publisherNode);
        }


        private static DeveloperEntry[] ExtractDevelopers(HtmlNode details)
        {
            var developerNodes = SelectNodeWithText(details, "Developed by")
                ?.NextSibling
                .SelectNodes("a");

            if (developerNodes == null)
            {
                return new DeveloperEntry[0];
            }

            return developerNodes.Select(ExtractNamedEntryFromNode<DeveloperEntry>).ToArray();
        }

        private static string[] ExtractPlatforms(HtmlNode details)
        {
            return SelectNodeWithTextStartingWith(details, "Platform")
                ?.NextSibling
                .SelectNodes("a")
                .Select(node => node.PlainInnerText())
                .ToArray();
        }

        private static string ExtractReleaseDate(HtmlNode details)
        {
            return SelectNodeWithText(details, "Released")
                ?.NextSibling
                .SelectSingleNode("a")
                .PlainInnerText();
        }
    }
}