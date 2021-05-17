﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
        private readonly IWebClient webClient;

        public Scraper(IWebClient webClient)
        {
            this.webClient = webClient;
        }

        private readonly Regex backgroundImageRegex = new Regex("background(?:-image)?:\\s*url\\('?(.*?)'?\\)", RegexOptions.Compiled);
        private readonly Regex releaseYearRegex = new Regex("^(.*?)\\s+\\([0-9]{4}\\)$", RegexOptions.Compiled);

        private const string SEARCH_RESULT = "searchResult";
        private const string SEARCH_TITLE = "searchTitle";
        private const string SEARCH_DETAILS = "searchDetails";

        private const string CORE_GAME_RELEASE_ID = "coreGameRelease";
        private const string GAME_NAME_TITLE = "niceHeaderTitle";

        private const string OFFICIAL_SCREENSHOTS_ID = "official_screenshots";
        private const string THUMBNAIL_GALLERY = "thumbnailGallery";

        public List<SearchResult> Search(string term)
        {
            var doc = webClient.Load(BuildSearchUrl(term));

            var results = doc.DocumentNode.SelectNodesByClass(SEARCH_RESULT, "div");


            var entries = results
                .Where(result =>
                    result.SelectSingleNodeByClass(SEARCH_TITLE)?.PlainInnerText().StartsWith("Game:") ?? false)
                .Select(result =>
                {
                    var entryLink = result.SelectSingleNodeByClass(SEARCH_TITLE).SelectSingleNode("a");

                    var searchResult = ExtractNamedEntryFromNode<SearchResult>(entryLink);

                    searchResult.Releases = result
                        .SelectSingleNodeByClass(SEARCH_DETAILS)
                        .SelectNodes("span")
                        .Select(sp =>
                        {
                            var platformLink = sp.SelectSingleNode("a");

                            if (platformLink == null)
                            {
                                var text = sp.PlainInnerText() ?? string.Empty;

                                var matches = releaseYearRegex.Matches(text);

                                return new SearchResult.Release
                                {
                                    Text = text,
                                    Platform = matches.Count > 0 ? matches[0].Value : text,
                                    Url = entryLink.GetAttributeValue("href", "")
                                };
                            }

                            return new SearchResult.Release
                            {
                                Text = sp.PlainInnerText(),
                                Platform = platformLink.PlainInnerText(),
                                Url = platformLink.GetAttributeValue("href", "")
                            };
                        })
                        .ToArray();

                    return searchResult;
                })
                .ToList();

            return entries;
        }

        public GameEntry GetGame(string url)
        {
            var doc = webClient.Load(url).DocumentNode;

            var details = doc.SelectSingleNodeById(CORE_GAME_RELEASE_ID);

            return new GameEntry
            {
                Name = doc.SelectSingleNodeByClass(GAME_NAME_TITLE).SelectSingleNode("a").PlainInnerText(),
                Slug = ExtractSlug(url),
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

            var doc = webClient.Load(url).DocumentNode;

            var platformTechInfos = doc
                .SelectNodesByClass("techInfo")
                .ToDictionary(node => node.SelectSingleNode("thead").PlainInnerText().Trim());

            var platforms = new List<string>();

            if (platformTechInfos.ContainsKey("Windows"))
            {
                var minimumOs = platformTechInfos["Windows"].SelectNodeWithText("Minimum OS Class Required")
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
                    pair.Value.SelectNodeWithText("Media Type")
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

            var doc = webClient.Load(url).DocumentNode;

            var officialScreenshotsNode = doc
                .SelectSingleNodeById(OFFICIAL_SCREENSHOTS_ID)
                ?.SelectFollowingNodeByClass(THUMBNAIL_GALLERY);

            var baseUri = new Uri(url);

            var officialScreenshots = ExtractOfficialScreenshots(baseUri, officialScreenshotsNode).ToArray();

            var screenshots = ExtractScreenshots(baseUri, doc.SelectNodesByClass("thumbnail-image"));

            return officialScreenshots.Concat(screenshots).ToArray();
        }

        public CoverArtCollectionEntry[] GetCoverArt(string slug)
        {
            var url = $"{BuildGameUrl(slug)}/cover-art";

            var doc = webClient.Load(url).DocumentNode;

            var baseUri = new Uri(url);

            return doc.SelectNodesByClass("coverHeading")
                .Select(cover => ExtractCoverArtCollectionEntry(baseUri, cover))
                .ToArray();
        }

        public async Task<ImageEntry> DownloadScreenshot(string url, IProgress<int> progress = null)
        {
            var doc = webClient.Load(url).DocumentNode;

            var container = doc.SelectSingleNodeByClassContains("screenshot") ?? doc.SelectSingleNodeByClass("promoImage");

            var src = container
                ?.SelectSingleNode(".//img")
                .GetAttributeValue("src", null);

            if (src == null)
            {
                throw new ScraperException($"Could not find screenshot in page: {url}");
            }

            var baseUrl = new Uri(url);
            var imageUrl = new Uri(baseUrl, src);

            using var client = new System.Net.WebClient();

            if (progress != null)
            {
                client.DownloadProgressChanged += (_, args) => { progress.Report(args.ProgressPercentage); };
            }

            var data = await client.DownloadDataTaskAsync(imageUrl);

            return new ImageEntry
            {
                Data = data,
                Url = imageUrl
            };
        }

        public async Task<ImageEntry> DownloadCoverArt(string url, IProgress<int> progress = null)
        {
            var doc = webClient.Load(url).DocumentNode;

            var container = doc.SelectSingleNodeById("main");

            var src = container
                ?.SelectSingleNode(".//h1/following::*//img")
                .GetAttributeValue("src", null);

            if (src == null)
            {
                throw new ScraperException($"Could not find cover art in page: {url}");
            }

            var baseUrl = new Uri(url);
            var imageUrl = new Uri(baseUrl, src);

            using var client = new System.Net.WebClient();

            if (progress != null)
            {
                client.DownloadProgressChanged += (_, args) => { progress.Report(args.ProgressPercentage); };
            }

            var data = await client.DownloadDataTaskAsync(imageUrl);

            return new ImageEntry
            {
                Data = data,
                Url = imageUrl
            };
        }

        private static IEnumerable<ScreenshotEntry> ExtractOfficialScreenshots(Uri baseUri, HtmlNode gallery)
        {
            if (gallery == null)
            {
                return new List<ScreenshotEntry>();
            }

            return gallery
                .SelectNodes(".//a[img]")
                .Select(thumbnail => new ScreenshotEntry
                {
                    Url = thumbnail.GetAttributeValue("href", null),
                    Thumbnail = new Uri(baseUri, thumbnail.SelectSingleNode("img").GetAttributeValue("src", null))
                        .ToString()
                });
        }

        private IEnumerable<ScreenshotEntry> ExtractScreenshots(Uri baseUri, HtmlNodeCollection nodes)
        {
            if (nodes == null)
            {
                return new List<ScreenshotEntry>();
            }

            return nodes
                .Select(thumbnail =>
                {
                    var thumbnailPath = backgroundImageRegex.Match(thumbnail.GetAttributeValue("style", ""))
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

            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("q", term);

            url.Query = queryString.ToString();

            return url.ToString();
        }

        private static string ExtractSlug(string href)
        {
            return new Uri(href).Segments.Last();
        }

        private static T ExtractNamedEntryFromNode<T>(HtmlNode node) where T : NamedEntry, new()
        {
            if (!(node is {Name: "a"}))
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
            var publisherNode = details.SelectNodeWithText("Published by")
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
            var developerNodes = details.SelectNodeWithText("Developed by")
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
            return details.SelectNodeWithTextStartingWith("Platform")
                ?.NextSibling
                .SelectNodes("a")
                .Select(node => node.PlainInnerText())
                .ToArray();
        }

        private static string ExtractReleaseDate(HtmlNode details)
        {
            return details.SelectNodeWithText("Released")
                ?.NextSibling
                .SelectSingleNode("a")
                .PlainInnerText();
        }

        private CoverArtCollectionEntry ExtractCoverArtCollectionEntry(Uri baseUri, HtmlNode coverCollection)
        {
            var platform = coverCollection.SelectSingleNode(".//h2").PlainInnerText();

            var country = coverCollection.SelectNodeWithText("Country")?.NextSibling?.NextSibling
                .PlainInnerText();

            var covers = coverCollection
                .SelectFollowingNodeByClass("row")
                .SelectNodesByClass("thumbnail")
                .Select(cover =>
                {

                    var caption = cover
                        .SelectSingleNodeByClass("thumbnail-cover-caption")
                        .PlainInnerText()
                        .Trim();

                    var thumbnail = cover.SelectSingleNodeByClass("thumbnail-cover") ??
                                    throw new ScraperException(
                                        $"Could not find cover art element with class \"thumbnail-cover\" in {platform}, {country}.");

                    var thumbnailUrl = backgroundImageRegex
                        .Match(thumbnail.GetAttributeValue("style", string.Empty))
                        ?.Groups[1]
                        .Value;

                    return new CoverArtEntry
                    {
                        Type = caption switch
                        {
                            "Front Cover" => CoverArtEntry.CoverArtType.Front,
                            "Back Cover" => CoverArtEntry.CoverArtType.Back,
                            var s when s.StartsWith("Media") => CoverArtEntry.CoverArtType.Media,
                            _ => CoverArtEntry.CoverArtType.Other,
                        },
                        Url = thumbnail.GetAttributeValue("href", string.Empty),
                        Thumbnail = new Uri(baseUri, thumbnailUrl).ToString(),
                    };
                });

            return new CoverArtCollectionEntry
            {
                Platform = platform,
                Country = country,
                Covers = covers.ToArray()
            };
        }
    }
}
