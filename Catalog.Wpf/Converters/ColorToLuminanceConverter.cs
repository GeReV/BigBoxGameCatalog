using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class ColorToLuminanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                System.Drawing.Color color => color.GetLuminance(),
                System.Windows.Media.Color color => color.GetLuminance(),
                _ => DependencyProperty.UnsetValue
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
