using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Catalog.Wpf
{
    public class FileIconValueConverter : IValueConverter
    {
        private ImageSource icon;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string file = value?.ToString();

            if (icon == null && file != null && File.Exists(file))
            {
                using (var nativeIcon = Icon.ExtractAssociatedIcon(file))
                {
                    if (nativeIcon == null)
                    {
                        return Application.Current.Resources.FindName("IconDocument") as BitmapSource;
                    }

                    icon = Imaging.CreateBitmapSourceFromHIcon(
                        nativeIcon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );
                }
            }

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}