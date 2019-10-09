using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class DictionaryAccessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dictionary = values[0] as IDictionary;
            var key = values[1];

            if (dictionary?.Contains(key) == true)
            {
                return dictionary[key];
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}