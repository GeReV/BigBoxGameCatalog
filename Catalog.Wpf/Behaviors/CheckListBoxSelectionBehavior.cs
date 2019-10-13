using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace Catalog.Wpf.Behaviors
{
    public class CheckListBoxSelectionBehavior : Behavior<CheckListBox>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList),
                typeof(CheckListBoxSelectionBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var behavior = (CheckListBoxSelectionBehavior) sender;
            if (behavior.modelHandled) return;

            if (behavior.AssociatedObject == null)
                return;

            behavior.modelHandled = true;
            behavior.SelectItems();
            behavior.modelHandled = false;
        }

        private bool viewHandled;
        private bool modelHandled;

        public IList SelectedItems
        {
            get => (IList) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        // Propagate selected items from model to view
        private void SelectItems()
        {
            viewHandled = true;
            AssociatedObject.SelectedItems.Clear();
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                    AssociatedObject.SelectedItems.Add(item);
            }

            viewHandled = false;
        }

        // Propagate selected items from view to model
        private void OnCheckListBoxSelectionChanged(object sender, ItemSelectionChangedEventArgs itemSelectionChangedEventArgs)
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
        private void OnListBoxItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
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

            AssociatedObject.ItemSelectionChanged += OnCheckListBoxSelectionChanged;
            ((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged += OnListBoxItemsChanged;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null)
            {
                return;
            }

            AssociatedObject.ItemSelectionChanged -= OnCheckListBoxSelectionChanged;
            ((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged -= OnListBoxItemsChanged;
        }
    }
}