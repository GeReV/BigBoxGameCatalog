using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Catalog.Wpf.ViewModel
{
    public class MainWindowViewMode
    {
        public static readonly MainWindowViewMode GalleryMode =
            new(
                (ImageSource)(Application.Current.FindResource("IconApplicationIconLarge") ??
                              throw new InvalidOperationException()),
                "Gallery View"
            );

        public static readonly MainWindowViewMode DetailsMode =
            new(
                (ImageSource)(Application.Current.FindResource("IconApplicationDetail") ??
                              throw new InvalidOperationException()),
                "Detail View"
            );

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
