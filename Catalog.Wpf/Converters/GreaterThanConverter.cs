using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class GreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IComparable comparable))
            {
                return DependencyProperty.UnsetValue;
            }

            if (!float.TryParse(parameter?.ToString(), out var param))
            {
                return DependencyProperty.UnsetValue;
            }

            return comparable.CompareTo(param) > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
