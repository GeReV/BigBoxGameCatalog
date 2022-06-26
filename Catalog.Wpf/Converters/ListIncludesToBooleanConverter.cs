using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Catalog.Model;

namespace Catalog.Wpf.Converters
{
    public class ListIncludesToBooleanConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is not IModel model)
            {
                return DependencyProperty.UnsetValue;
            }

            if (values[1] is not IList list)
            {
                return DependencyProperty.UnsetValue;
            }

            if (list.OfType<IModel>().Count() == list.Count)
            {
                return list
                    .OfType<IModel>()
                    .ToList()
                    .Exists(item => model.GetType() == item.GetType() && item.Id == model.Id);
            }

            if (list.OfType<IList>().Count() == list.Count)
            {
                return list
                    .OfType<IList>()
                    .All(
                        sublist =>
                            sublist
                                .OfType<IModel>()
                                .ToList()
                                .Exists(item => model.GetType() == item.GetType() && item.Id == model.Id)
                    );
            }

            return list.Contains(values[0]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
