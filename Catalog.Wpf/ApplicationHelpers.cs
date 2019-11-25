using System.Windows;
using Catalog.Scrapers;

namespace Catalog.Wpf
{
    public static class ApplicationHelpers
    {
        public static string HomeDirectory(this Application application) =>
            application.Properties[nameof(HomeDirectory)].ToString();

        public static CatalogContext Database(this Application application) =>
            application.Properties[nameof(Database)] as CatalogContext;

        public static IWebClient ScraperWebClient(this Application application) =>
            (IWebClient) application.Properties[nameof(ScraperWebClient)];
    }
}
