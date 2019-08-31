using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;

namespace Catalog.Scrapers.MobyGames
{
    public class MobyGamesScraper
    {
        public MobyGamesScraper()
        {
        }

        public List<GameEntry> Search(string term)
        {
            var url = new UriBuilder("https://www.mobygames.com/search/quick");

            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("q", term);

            url.Query = queryString.ToString();

            var doc = LoadUrl(url.ToString());

            var results = doc.DocumentNode.SelectNodesByClass("searchResult", "div");

            var entries = results
                .Where(result => result.SelectSingleNodeByClass("searchTitle")?.InnerText.StartsWith("Game:") ?? false)
                .Select(result =>
                {
                    var entryLink = result.SelectSingleNodeByClass("searchTitle").SelectSingleNode("a");

                    return new GameEntry
                    {
                        Name = entryLink.InnerText,
                        Href = entryLink.GetAttributeValue("href", string.Empty),
                        Releases = result.SelectSingleNodeByClass("searchDetails").SelectNodes("span").Select(sp => sp.InnerText).ToArray(),
                    };
                })
                .ToList();

            return entries;
        }

        private HtmlDocument LoadUrl(string url)
        {
            var web = new HtmlWeb();
            return web.Load(url);
        }
    }
}
