using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Catalog.Wpf.Behaviors
{
    /// <summary>
    ///     Taken from: https://tyrrrz.me/blog/wpf-listbox-selecteditems-twoway-binding
    /// </summary>
    public class ListViewSelectionBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(ListViewSelectionBehavior),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged
                )
            );

        private bool modelHandled;

        private bool viewHandled;

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var behavior = (ListViewSelectionBehavior)sender;

            if (behavior.modelHandled)
            {
                return;
            }

            if (behavior.AssociatedObject == null)
            {
                return;
            }

            behavior.modelHandled = true;

            behavior.SelectItems();

            behavior.modelHandled = false;
        }

        // Propagate selected items from model to view
        private void SelectItems()
        {
            viewHandled = true;

            AssociatedObject.SelectedItems.Clear();

            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    AssociatedObject.SelectedItems.Add(item);
                }
            }

            viewHandled = false;
        }

        // Propagate selected items from view to model
        private void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (viewHandled)
            {
                return;
            }

            if (AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            SelectedItems = AssociatedObject.SelectedItems.Cast<object>().ToArray();
        }

        // Re-select items when the set of items changes
        private void OnListViewItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (viewHandled)
            {
                return;
            }

            if (AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            SelectItems();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnListViewSelectionChanged;
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += OnListViewItemsChanged;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null)
            {
                return;
            }

            AssociatedObject.SelectionChanged -= OnListViewSelectionChanged;

            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= OnListViewItemsChanged;
        }
    }
}
