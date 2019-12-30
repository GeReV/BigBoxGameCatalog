using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class UriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string s))
            {
                return DependencyProperty.UnsetValue;
            }

            return new Uri(s);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
