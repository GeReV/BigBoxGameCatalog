using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.Extensions.DependencyModel;

namespace Catalog.Wpf.Converters
{
    public class BooleanNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return !(bool)value;
        }
    }
}
