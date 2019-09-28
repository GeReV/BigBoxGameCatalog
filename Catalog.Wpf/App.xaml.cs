using System;
using System.IO;
using System.Reflection;

namespace Catalog.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var homeDirectory = Path.Combine(settingsDirectory, Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title);

            if (!Directory.Exists(homeDirectory))
            {
                Directory.CreateDirectory(homeDirectory);
            }

            Current.Properties.Add("Database", new CatalogDatabase(Path.Combine(homeDirectory, "database.litedb")));
        }
    }
}