using System.Windows;
using System.Windows.Controls;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class AddRemoveButtons : UserControl
    {
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