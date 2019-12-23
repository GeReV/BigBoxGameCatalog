using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.Extensions.DependencyModel;

namespace Catalog.Wpf.Converters
{
    public class DictionaryAccessConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!(values[0] is IDictionary dictionary))
            {
                return DependencyProperty.UnsetValue;
            }

            var key = values[1];

            if (dictionary.Contains(key))
            {
                return dictionary[key];
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
