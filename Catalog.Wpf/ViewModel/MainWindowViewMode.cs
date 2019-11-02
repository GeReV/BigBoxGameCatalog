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
        public static MainWindowViewMode GalleryMode = new MainWindowViewMode
        {
            IconSource = (ImageSource) Application.Current.FindResource("IconApplicationIconLarge"),
            Name = "Gallery View"
        };

        public static MainWindowViewMode DetailsMode = new MainWindowViewMode
        {
            IconSource = (ImageSource) Application.Current.FindResource("IconApplicationDetail"),
            Name = "Detail View"
        };

        public static IEnumerable<MainWindowViewMode> Modes => typeof(MainWindowViewMode)
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(fi => fi.FieldType == typeof(MainWindowViewMode))
            .Select(fi => fi.GetValue(null))
            .Cast<MainWindowViewMode>();

        public ImageSource IconSource { get; set; }
        public string Name { get; set; }
    }
}