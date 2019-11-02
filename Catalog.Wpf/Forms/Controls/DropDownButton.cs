using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Catalog.Wpf.Forms.Controls
{
    public class DropDownButton : ToggleButton
    {
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(nameof(Menu),
            typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null, OnMenuChanged));

        public DropDownButton()
        {
            // Bind the ToogleButton.IsChecked property to the drop-down's IsOpen property
            var binding = new Binding($"{nameof(Menu)}.{nameof(Menu.IsOpen)}")
            {
                Source = this
            };

            SetBinding(IsCheckedProperty, binding);

            DataContextChanged += (sender, args) =>
            {
                if (Menu != null)
                {
                    Menu.DataContext = DataContext;
                }
            };
        }

        // *** Properties ***
        public ContextMenu Menu
        {
            get => (ContextMenu) GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        private static void OnMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dropDownButton = (DropDownButton) d;
            var contextMenu = (ContextMenu) e.NewValue;
            contextMenu.DataContext = dropDownButton.DataContext;
        }

        protected override void OnClick()
        {
            if (Menu == null)
            {
                return;
            }

            Menu.PlacementTarget = this;
            Menu.Placement = PlacementMode.Bottom;
            Menu.IsOpen = true;
        }
    }
}