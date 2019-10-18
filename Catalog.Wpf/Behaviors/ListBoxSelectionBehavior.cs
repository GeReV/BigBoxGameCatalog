using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Eto.Wpf.CustomControls;
using Microsoft.Xaml.Behaviors;

namespace Catalog.Wpf.Behaviors
{
    /// <summary>
    /// Taken from: https://tyrrrz.me/blog/wpf-listbox-selecteditems-twoway-binding
    /// </summary>
    public class ListBoxSelectionBehavior : Behavior<ListBox>
    {
        private bool modelHandled;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(ListBoxSelectionBehavior),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    PropertyChangedCallback
                )
            );

        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            void Handler(object s, NotifyCollectionChangedEventArgs e) => SelectedItemsChanged(sender, e);

            if (args.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= Handler;
            }

            if (args.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += Handler;
            }

            if (args.NewValue == null)
            {
                return;
            }

            ResetSelectedItems(sender, (IEnumerable)args.NewValue);
        }

        public IList SelectedItems
        {
            get => (IList) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private static void ResetSelectedItems(object sender, IEnumerable items)
        {
            if (!(sender is ListBoxSelectionBehavior behavior))
            {
                return;
            }

            var listViewBase = behavior.AssociatedObject;

            var listSelectedItems = listViewBase.SelectedItems;

            listSelectedItems.Clear();

            foreach (var item in items)
            {
                listSelectedItems.Add(item);
            }
        }

        private static void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ListBoxSelectionBehavior behavior))
            {
                return;
            }

            var listViewBase = behavior.AssociatedObject;

            var listSelectedItems = listViewBase.SelectedItems;

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (listSelectedItems.Contains(item))
                    {
                        listSelectedItems.Remove(item);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (!listSelectedItems.Contains(item))
                    {
                        listSelectedItems.Add(item);
                    }
                }
            }
        }

        // Propagate selected items from view to model
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modelHandled)
            {
                return;
            }

            modelHandled = true;

            foreach (var item in e.RemovedItems)
            {
                if (SelectedItems.Contains(item))
                {
                    SelectedItems.Remove(item);
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (!SelectedItems.Contains(item))
                {
                    SelectedItems.Add(item);
                }
            }

            modelHandled = false;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }
    }
}