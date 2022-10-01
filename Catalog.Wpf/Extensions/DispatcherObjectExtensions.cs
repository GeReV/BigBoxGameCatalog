using System.Windows;
using System.Windows.Media;

namespace Catalog.Wpf.Extensions
{
    public static class DispatcherObjectExtensions
    {
        public static T? FindAncestorOrSelf<T>(this DependencyObject source)
            where T : DependencyObject
        {
            var obj = source;

            while (obj != null)
            {
                if (obj is T target)
                {
                    return target;
                }

                obj = GetParent(obj);
            }

            return null;
        }

        public static DependencyObject? GetParent(this DependencyObject obj)
        {
            switch (obj)
            {
                case null:
                    return null;
                case ContentElement ce:
                {
                    if (ContentOperations.GetParent(ce) is { } parent)
                    {
                        return parent;
                    }

                    return ce is FrameworkContentElement fce ? fce.Parent : null;
                }
                default:
                    return VisualTreeHelper.GetParent(obj);
            }
        }
    }
}
