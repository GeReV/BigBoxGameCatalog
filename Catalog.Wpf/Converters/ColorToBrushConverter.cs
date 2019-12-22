using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Catalog.Wpf.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                Color color => new SolidColorBrush(color),
                System.Drawing.Color color => new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)),
                _ => DependencyProperty.UnsetValue,
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush brush))
            {
                return DependencyProperty.UnsetValue;
            }

            return Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
        }
    }
}
