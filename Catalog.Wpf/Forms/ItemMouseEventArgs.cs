using System.Windows;

namespace Catalog.Wpf.Forms
{
    public class ItemMouseEventArgs : RoutedEventArgs
    {
        public object Item { get; }
        public int ItemIndex { get; }

        public ItemMouseEventArgs(RoutedEvent routedEvent, object item, int itemIndex) : base(routedEvent)
        {
            Item = item;
            ItemIndex = itemIndex;
        }
    }
}
