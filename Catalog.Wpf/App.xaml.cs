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
            var settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var homeDirectory = Path.Combine(settingsDirectory, Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title);

            if (!Directory.Exists(homeDirectory))
            {
                Directory.CreateDirectory(homeDirectory);
            }

            Current.Properties.Add(nameof(ApplicationHelpers.HomeDirectory), homeDirectory);
            Current.Properties.Add(nameof(ApplicationHelpers.Database), new CatalogDatabase(Path.Combine(homeDirectory, "database.litedb")));
            Current.Properties.Add(nameof(ApplicationHelpers.ScraperWebClient), new CachingWebClient());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.Database().Dispose();
        }
    }
}