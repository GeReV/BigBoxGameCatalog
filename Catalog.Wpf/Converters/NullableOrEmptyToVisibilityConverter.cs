using System;
using System.Globalization;
using System.Windows.Data;

namespace Catalog.Wpf.Converters
{
    public class NullableOrEmptyToVisibilityConverter : IValueConverter
    {
        private readonly IValueConverter nullableOrEmptyToBooleanConverter = new NullableOrEmptyToBooleanConverter();
        private readonly IValueConverter booleanToVisibilityConverter = new BooleanToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return booleanToVisibilityConverter.Convert(
                nullableOrEmptyToBooleanConverter.Convert(value, typeof(object), parameter, culture),
                targetType,
                parameter,
                culture
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}