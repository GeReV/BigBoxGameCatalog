using System.Linq;
using HtmlAgilityPack;

namespace Catalog.Scrapers.MobyGames
{
    public static class HtmlDocumentHelpers
    {
        private static string ClassSelector(string className, string? tag = null) =>
            $"{tag ?? "*"}[@class='{className}']";

        private static string ClassContainsSelector(string className, string? tag = null) =>
            // Source: https://devhints.io/xpath
            $"{tag ?? "*"}[contains(concat(' ',normalize-space(@class),' '),' {className} ')]";

        public static HtmlNodeCollection? SelectNodesByClass(this HtmlNode node, string className, string? tag = null)
        {
            return node.SelectNodes(".//" + ClassSelector(className, tag));
        }

        public static HtmlNode? SelectSingleNodeByClass(this HtmlNode node, string className, string? tag = null)
        {
            return node.SelectSingleNode(".//" + ClassSelector(className, tag));
        }

        public static HtmlNode? SelectSingleNodeByClassContains(this HtmlNode node, string className, string? tag = null)
        {
            return node.SelectSingleNode(".//" + ClassContainsSelector(className, tag));
        }

        public static HtmlNode? SelectSingleNodeById(this HtmlNode node, string id)
        {
            return node.SelectSingleNode($".//*[@id='{id}']");
        }

        public static HtmlNode? SelectFollowingNode(this HtmlNode node, string selector)
        {
            return node.SelectSingleNode($"./following-sibling::{selector}");
        }

        public static HtmlNode? SelectFollowingNodeByTagName(this HtmlNode node, string tag)
        {
            return node.SelectFollowingNode(tag);
        }

        public static HtmlNode? SelectFollowingNodeByClass(this HtmlNode node, string className, string? tag = null)
        {
            return node.SelectFollowingNode(ClassSelector(className, tag));
        }

        public static HtmlNode? SelectFollowingNodeById(this HtmlNode node, string id)
        {
            return node.SelectFollowingNode($"*[@id='{id}']");
        }

        public static HtmlNode? SelectNodeWithText(this HtmlNode details, string title)
        {
            return details
                .SelectNodes($".//*")
                .FirstOrDefault(node => node.PlainInnerText() == title);
        }

        public static HtmlNode? SelectNodeWithTextStartingWith(this HtmlNode details, string title)
        {
            return details
                .SelectNodes(".//*")
                .FirstOrDefault(node => node.PlainInnerText().StartsWith(title));
        }

        public static string PlainInnerText(this HtmlNode node)
        {
            return NormalizeWhitespace(HtmlEntity.DeEntitize(node.InnerText));
        }

        private static string NormalizeWhitespace(string s)
        {
            return s.Replace('\u00A0', ' ');
        }
    }
}
