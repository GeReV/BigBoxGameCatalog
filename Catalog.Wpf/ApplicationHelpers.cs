using System.Windows;

namespace Catalog.Wpf
{
    public static class ApplicationHelpers
    {
        public static CatalogDatabase Database(this Application application) =>
            application.Properties["Database"] as CatalogDatabase;
    }
}