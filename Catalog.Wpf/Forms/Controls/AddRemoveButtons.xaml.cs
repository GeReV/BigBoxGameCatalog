using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class AddRemoveButtons : UserControl
    {
        public static readonly DependencyProperty ShowAddButtonProperty = DependencyProperty.Register(
            nameof(ShowAddButton), typeof(bool), typeof(AddRemoveButtons), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowRemoveButtonProperty = DependencyProperty.Register(
            nameof(ShowRemoveButton), typeof(bool), typeof(AddRemoveButtons), new PropertyMetadata(true));

        public bool ShowRemoveButton
        {
            get => (bool) GetValue(ShowRemoveButtonProperty);
            set => SetValue(ShowRemoveButtonProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register(
            nameof(AddCommand), typeof(ICommand), typeof(AddRemoveButtons), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty AddCommandParameterProperty = DependencyProperty.Register(
            nameof(AddCommandParameter), typeof(object), typeof(AddRemoveButtons),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
            nameof(RemoveCommand), typeof(ICommand), typeof(AddRemoveButtons), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty RemoveCommandParameterProperty = DependencyProperty.Register(
            nameof(RemoveCommandParameter), typeof(object), typeof(AddRemoveButtons),
            new PropertyMetadata(default(object)));

        public bool ShowAddButton
        {
            get => (bool) GetValue(ShowAddButtonProperty);
            set => SetValue(ShowAddButtonProperty, value);
        }

        public ICommand AddCommand
        {
            get => (ICommand) GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public object AddCommandParameter
        {
            get => GetValue(AddCommandParameterProperty);
            set => SetValue(AddCommandParameterProperty, value);
        }

        public ICommand RemoveCommand
        {
            get => (ICommand) GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public object RemoveCommandParameter
        {
            get => GetValue(RemoveCommandParameterProperty);
            set => SetValue(RemoveCommandParameterProperty, value);
        }

        public AddRemoveButtons()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler AddClick;
        public event RoutedEventHandler RemoveClick;

        protected virtual void OnAddClick(RoutedEventArgs e)
        {
            AddClick?.Invoke(this, e);
        }

        protected virtual void OnRemoveClick(RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, e);
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            OnAddClick(e);
        }

        private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
        {
            OnRemoveClick(e);
        }
    }
}
