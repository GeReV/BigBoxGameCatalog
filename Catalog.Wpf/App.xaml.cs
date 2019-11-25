using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Catalog.Scrapers;

namespace Catalog.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            var homeDirectory = EnsureHomeDirectory();

            Current.Properties.Add(nameof(ApplicationHelpers.HomeDirectory), homeDirectory);
            Current.Properties.Add(nameof(ApplicationHelpers.Database), InitializeDatabase(homeDirectory));
            Current.Properties.Add(nameof(ApplicationHelpers.ScraperWebClient), new CachingWebClient());
        }

        private static string EnsureHomeDirectory()
        {
            var settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var homeDirectory = Path.Combine(settingsDirectory,
                Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title);

            if (!Directory.Exists(homeDirectory))
            {
                Directory.CreateDirectory(homeDirectory);
            }

            return homeDirectory;
        }

        private static CatalogContext InitializeDatabase(string homeDirectory)
        {
            var context = new CatalogContext(Path.Combine(homeDirectory, "database.sqlite"));

            context.Database.EnsureCreated();

            return context;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.Database().Dispose();
        }
    }
}
