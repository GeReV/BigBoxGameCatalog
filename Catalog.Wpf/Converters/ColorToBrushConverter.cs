using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Catalog.Wpf.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color color))
            {
                return DependencyProperty.UnsetValue;
            }

            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

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
