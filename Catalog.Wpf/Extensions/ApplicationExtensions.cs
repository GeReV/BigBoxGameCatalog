using System;
using System.IO;
using System.Windows;
using MobyGames.API;

namespace Catalog.Wpf.Extensions
{
    public static class ApplicationHelpers
    {
        public static string HomeDirectory(this Application application) =>
            application.Properties[nameof(HomeDirectory)]?.ToString() ??
            throw new NullReferenceException("Expected a home directory to be set.");

        public static CatalogContext Database(this Application application) =>
            new(Path.Combine(application.HomeDirectory(), "database.sqlite"));

        public static MobyGamesClient MobyGamesClient(this Application application)
        {
            application.Properties[nameof(MobyGamesClient)] ??=
                new MobyGamesClient(AppSettingsHelper.Current.MobyGamesApiKey());

            return (MobyGamesClient)application.Properties[nameof(MobyGamesClient)]!;
        }
    }
}
