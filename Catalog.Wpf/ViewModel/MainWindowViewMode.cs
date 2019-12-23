using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Catalog.Wpf.ViewModel
{
    public class MainWindowViewMode
    {
        public static MainWindowViewMode GalleryMode =
            new MainWindowViewMode((ImageSource) Application.Current.FindResource("IconApplicationIconLarge"),
                "Gallery View");

        public static MainWindowViewMode DetailsMode =
            new MainWindowViewMode((ImageSource) Application.Current.FindResource("IconApplicationDetail"),
                "Detail View");

        public static IEnumerable<MainWindowViewMode> Modes => typeof(MainWindowViewMode)
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(fi => fi.FieldType == typeof(MainWindowViewMode))
            .Select(fi => fi.GetValue(null))
            .Cast<MainWindowViewMode>();

        public MainWindowViewMode(ImageSource iconSource, string name)
        {
            IconSource = iconSource;
            Name = name;
        }

        public ImageSource IconSource { get; set; }
        public string Name { get; set; }
    }
}
