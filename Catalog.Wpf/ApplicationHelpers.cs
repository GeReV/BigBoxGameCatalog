using System;
using System.IO;
using System.Windows;
using Catalog.Scrapers;

namespace Catalog.Wpf
{
    public static class ApplicationHelpers
    {
        public static string HomeDirectory(this Application application) =>
            application.Properties[nameof(HomeDirectory)]?.ToString() ?? throw new NullReferenceException("Expected a home directory to be set.");

        public static CatalogContext Database(this Application application) =>
            new CatalogContext(Path.Combine(application.HomeDirectory(), "database.sqlite"));

        public static IWebClient ScraperWebClient(this Application application) => (IWebClient)(application.Properties[nameof(ScraperWebClient)] ?? throw new NullReferenceException("Expected a ScraperWebClient."));
    }
}
