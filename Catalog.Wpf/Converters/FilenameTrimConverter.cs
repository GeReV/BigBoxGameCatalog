using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class FilenameTrimConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string s))
            {
                return DependencyProperty.UnsetValue;
            }

            return s.Substring(s.LastIndexOfAny(new [] { '/', '\\' }) + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}