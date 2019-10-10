using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class AddRemoveButtons : UserControl
    {
        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register(
            "AddCommand", typeof(ICommand), typeof(AddRemoveButtons), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty AddCommandParameterProperty = DependencyProperty.Register(
            "AddCommandParameter", typeof(object), typeof(AddRemoveButtons), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
            "RemoveCommand", typeof(ICommand), typeof(AddRemoveButtons), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty RemoveCommandParameterProperty = DependencyProperty.Register(
            "RemoveCommandParameter", typeof(object), typeof(AddRemoveButtons), new PropertyMetadata(default(object)));


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