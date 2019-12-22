using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Catalog.Wpf.Converters
{
    public class BrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush brush))
            {
                return DependencyProperty.UnsetValue;
            }

            return brush.Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color color))
            {
                return DependencyProperty.UnsetValue;
            }

            return new SolidColorBrush(color);
        }
    }
}
