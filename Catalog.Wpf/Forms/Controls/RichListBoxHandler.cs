using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Catalog.Forms.Controls;
using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;

namespace Catalog.Wpf.Forms.Controls
{
    public class EtoRichListBox : swc.ListBox, IEtoWpfControl
    {
        public IWpfFrameworkElement Handler { get; set; }

        protected override sw.Size MeasureOverride(sw.Size constraint)
        {
            return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
        }
    }

    public class RichListBoxHandler : WpfControl<swc.ListBox, RichListBox, RichListBox.ICallback>, RichListBox.IHandler
    {
        private IEnumerable<object> store;
        private ContextMenu contextMenu;

        protected override sw.Size DefaultSize => new sw.Size(100, 120);

        public RichListBoxHandler()
        {
            Control = new EtoRichListBox
            {
                HorizontalAlignment = sw.HorizontalAlignment.Stretch,
                Handler = this
            };

            var template = new sw.DataTemplate
            {
                VisualTree = new WpfImageTextSubtitleBindingBlock(() => Widget.ItemTextBinding, () => Widget.ItemSubtitleBinding, () => Widget.ItemImageBinding, false),
            };

            Control.ItemTemplate = template;
            Control.SelectionChanged += delegate
            {
                Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
            };
            Control.MouseDoubleClick += delegate
            {
                if (SelectedIndex >= 0)
                    Callback.OnActivated(Widget, EventArgs.Empty);
            };
            Control.KeyDown += (sender, e) =>
            {
                if (e.Key == sw.Input.Key.Return)
                {
                    if (SelectedIndex >= 0)
                    {
                        Callback.OnActivated(Widget, EventArgs.Empty);
                        e.Handled = true;
                    }
                }
            };
        }

        public override void Focus()
        {
            if (Control.IsLoaded)
            {
                if (Control.ItemContainerGenerator.ContainerFromIndex(Math.Max(0, SelectedIndex)) is sw.FrameworkElement item)
                    item.Focus();
                else
                    Control.Focus();
            }
            else
            {
                Control.Loaded += Control_Loaded;
            }
        }

        private void Control_Loaded(object sender, sw.RoutedEventArgs e)
        {
            Focus();
            Control.Loaded -= Control_Loaded;
        }

        public override bool UseMousePreview { get { return true; } }

        public override bool UseKeyPreview { get { return true; } }

        public IEnumerable<object> DataStore
        {
            get { return store; }
            set {
                store = value;
                var source = store as IEnumerable<object>;
                if (source != null && !(source is INotifyCollectionChanged))
                    source = new ObservableCollection<object>(source);
                Control.ItemsSource = source;
            }
        }

        public int SelectedIndex
        {
            get { return Control.SelectedIndex; }
            set {
                Control.SelectedIndex = value;
                if (value >= 0 && store != null)
                {
                    var item = store.AsEnumerable().Skip(value).FirstOrDefault();
                    Control.ScrollIntoView(item);
                }
            }
        }

        public ContextMenu ContextMenu
        {
            get { return contextMenu; }
            set {
                contextMenu = value;
                Control.ContextMenu = contextMenu != null ? contextMenu.ControlObject as sw.Controls.ContextMenu : null;
            }
        }

        private IIndirectBinding<string> _itemTextBinding;
        public IIndirectBinding<string> ItemTextBinding
        {
            get => _itemTextBinding;
            set {
                _itemTextBinding = value;
                Control.InvalidateVisual();
            }
        }

        private IIndirectBinding<string> _itemSubtitleBinding;
        public IIndirectBinding<string> ItemSubtitleBinding
        {
            get => _itemSubtitleBinding;
            set {
                _itemSubtitleBinding = value;
                Control.InvalidateVisual();
            }
        }
        public IIndirectBinding<string> ItemKeyBinding { get; set; }
    }
}
