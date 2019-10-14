using System;
using System.IO;
using System.Reflection;
using System.Windows;

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

            Current.Properties.Add("HomeDirectory", homeDirectory);
            Current.Properties.Add("Database", new CatalogDatabase(Path.Combine(homeDirectory, "database.litedb")));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.Database().Dispose();
        }
    }
}