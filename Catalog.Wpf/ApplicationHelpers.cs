using System.Windows;

namespace Catalog.Wpf
{
    public static class ApplicationHelpers
    {
        public static string HomeDirectory(this Application application) =>
            application.Properties["HomeDirectory"].ToString();
        public static CatalogDatabase Database(this Application application) =>
            application.Properties["Database"] as CatalogDatabase;
    }
}