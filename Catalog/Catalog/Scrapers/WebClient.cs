using HtmlAgilityPack;

namespace Catalog.Scrapers
{
    public class WebClient : IWebClient
    {
        public HtmlDocument Load(string url) => new HtmlWeb().Load(url);
    }
}