using System;
using System.Globalization;
using System.Windows.Data;

namespace Catalog.Wpf
{
    public class ByteArrayHexValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is byte[] bytes))
            {
                return string.Empty;
            }

            return BitConverter
                .ToString(bytes)
                .Replace("-", string.Empty)
                .ToLowerInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}