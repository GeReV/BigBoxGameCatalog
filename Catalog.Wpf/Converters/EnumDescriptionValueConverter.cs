using System;
using System.Globalization;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class EnumDescriptionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum e = (Enum) value;

            return e.GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}