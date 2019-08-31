using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames
{
    public static class HtmlDocumentHelpers
    {
        private static string classSelector(string className, string tag = null)
        {
            return string.Format(".//{0}[@class='{1}']", tag ?? "*", className);
        }

        public static HtmlNodeCollection SelectNodesByClass(this HtmlNode node, string className, string tag = null)
        {
            return node.SelectNodes(classSelector(className, tag));
        }

        public static HtmlNode SelectSingleNodeByClass(this HtmlNode node, string className, string tag = null)
        {
            return node.SelectSingleNode(classSelector(className, tag));
        }
    }
}
