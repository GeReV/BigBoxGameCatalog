using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.Converters
{
    public class ListIncludesToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[1] is IList list))
            {
                return DependencyProperty.UnsetValue;
            }

            if (values[0] is IModel model)
            {
                return list
                    .OfType<IModel>()
                    .ToList()
                    .Exists(item => model.GetType() == item.GetType() && item.Id == model.Id);
            }

            return list.Contains(values[0]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
