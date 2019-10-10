using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Hidden;
            }

            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility) (value ?? Visibility.Hidden);

            return visibility == Visibility.Visible;
        }
    }
}