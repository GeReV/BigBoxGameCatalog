using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class PropertyPathConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return value;
            }

            var properties = parameter.ToString().Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (value is IList list)
            {
                var result = new ArrayList(list.Count);

                foreach (var obj in list)
                {
                    result.Add(TraversePath(obj, properties));
                }

                return result;
            }

            return TraversePath(value, properties) ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static object TraversePath(object? value, IEnumerable<string> properties)
        {
            var result = value;

            foreach (var propertyName in properties)
            {
                if (result == null)
                {
                    return DependencyProperty.UnsetValue;
                }

                var property = result.GetType().GetProperty(propertyName);

                if (property == null)
                {
                    return DependencyProperty.UnsetValue;
                }

                result = property.GetValue(result);
            }

            return result;
        }
    }
}
