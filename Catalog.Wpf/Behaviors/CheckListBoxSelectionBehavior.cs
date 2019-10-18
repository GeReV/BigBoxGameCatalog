using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace Catalog.Wpf.Behaviors
{
    public class CheckListBoxSelectionBehavior : Behavior<CheckListBox>
    {
        private bool modelHandled;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(CheckListBoxSelectionBehavior),
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

        private static void ResetSelectedItems(object sender, IEnumerable items)
        {
            if (!(sender is CheckListBoxSelectionBehavior behavior))
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


        public IList SelectedItems
        {
            get => (IList) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private static void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is CheckListBoxSelectionBehavior behavior))
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
        private void OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (modelHandled)
            {
                return;
            }

            modelHandled = true;

            if (e.IsSelected)
            {
                if (!SelectedItems.Contains(e.Item))
                {
                    SelectedItems.Add(e.Item);
                }
            }
            else
            {
                if (SelectedItems.Contains(e.Item))
                {
                    SelectedItems.Remove(e.Item);
                }
            }

            modelHandled = false;
        }
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.ItemSelectionChanged += OnSelectionChanged;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.ItemSelectionChanged += OnSelectionChanged;
        }
    }
}