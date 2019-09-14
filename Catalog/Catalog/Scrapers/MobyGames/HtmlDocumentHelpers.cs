using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Catalog.Scrapers.MobyGames
{
    public static class HtmlDocumentHelpers
    {
        private static string classSelector(string className, string tag = null)
        {
            return $"{tag ?? "*"}[@class='{className}']";
        }

        public static HtmlNodeCollection SelectNodesByClass(this HtmlNode node, string className, string tag = null)
        {
            return node.SelectNodes(".//" + classSelector(className, tag));
        }

        public static HtmlNode SelectSingleNodeByClass(this HtmlNode node, string className, string tag = null)
        {
            return node.SelectSingleNode(".//" + classSelector(className, tag));
        }

        public static HtmlNode SelectSingleNodeById(this HtmlNode node, string id)
        {
            return node.SelectSingleNode($".//*[@id='{id}']");
        }

        public static HtmlNode SelectFollowingNode(this HtmlNode node, string selector)
        {
            return node.SelectSingleNode($"./following-sibling::{selector}");
        }
        
        public static HtmlNode SelectFollowingNodeByTagName(this HtmlNode node, string tag)
        {
            return node.SelectFollowingNode(tag);
        }
        
        public static HtmlNode SelectFollowingNodeByClass(this HtmlNode node, string className, string tag = null)
        {
            return node.SelectFollowingNode(classSelector(className, tag));
        }
        
        public static HtmlNode SelectFollowingNodeById(this HtmlNode node, string id)
        {
            return node.SelectFollowingNode($"*[@id='{id}']");
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
