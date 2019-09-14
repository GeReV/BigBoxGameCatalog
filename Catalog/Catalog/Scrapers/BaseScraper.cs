using System.Linq;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using HtmlAgilityPack;

namespace Catalog.Scrapers
{
    public class BaseScraper
    {

        protected static HtmlNode SelectNodeWithText(HtmlNode details, string title)
        {
            return details
                .SelectNodes($".//*")
                .First(node => node.PlainInnerText() == title);
        }

        protected static HtmlNode SelectNodeWithTextStartingWith(HtmlNode details, string title)
        {
            return details
                .SelectNodes(".//*")
                .First(node => node.PlainInnerText().StartsWith(title));
        }
    }
}