using HtmlAgilityPack;

namespace Catalog.Scrapers
{
    public interface IWebClient
    {
        HtmlDocument Load(string url);
    }
}