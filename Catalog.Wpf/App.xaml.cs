using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Catalog.Scrapers;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string FALLBACK_DIRECTORY_NAME = "BBGC";

        public App()
        {
            var homeDirectory = EnsureHomeDirectory();

            Current.Properties.Add(nameof(ApplicationHelpers.HomeDirectory), homeDirectory);
            Current.Properties.Add(nameof(ApplicationHelpers.ScraperWebClient), new WebClient());

            InitializeDatabase();
        }

        private static string EnsureHomeDirectory()
        {
            var settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var homeDirectory = Path.Combine(settingsDirectory,
                Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? FALLBACK_DIRECTORY_NAME);

            if (!Directory.Exists(homeDirectory))
            {
                Directory.CreateDirectory(homeDirectory);
            }

            return homeDirectory;
        }

        private static void InitializeDatabase()
        {
            var context = Current.Database();

            context.Database.Migrate();
            context.Database.EnsureCreated();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.Database().Dispose();
        }
    }
}
