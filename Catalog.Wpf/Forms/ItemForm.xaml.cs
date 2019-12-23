using System.Windows;
using System.Windows.Controls;

namespace Catalog.Wpf.Forms
{
    public partial class ItemForm : UserControl
    {
        public event RoutedEventHandler? AddScanClick;
        public event RoutedEventHandler? RemoveScanClick;

        public event RoutedEventHandler? AddFileClick;
        public event RoutedEventHandler? RemoveFileClick;

        public ItemForm()
        {
            InitializeComponent();
        }

        protected virtual void OnAddScanClick(RoutedEventArgs e)
        {
            AddScanClick?.Invoke(this, e);
        }

        protected virtual void OnRemoveScanClick(RoutedEventArgs e)
        {
            RemoveScanClick?.Invoke(this, e);
        }

        protected virtual void OnAddFileClick(RoutedEventArgs e)
        {
            AddFileClick?.Invoke(this, e);
        }

        protected virtual void OnRemoveFileClick(RoutedEventArgs e)
        {
            RemoveFileClick?.Invoke(this, e);
        }

        private void AddRemoveFile_OnAddClick(object sender, RoutedEventArgs e)
        {
            OnAddFileClick(e);
        }

        private void AddRemoveFile_OnRemoveClick(object sender, RoutedEventArgs e)
        {
            OnRemoveFileClick(e);
        }

        private void AddRemoveImage_OnAddClick(object sender, RoutedEventArgs e)
        {
            OnAddScanClick(e);
        }

        private void AddRemoveImage_OnRemoveClick(object sender, RoutedEventArgs e)
        {
            OnRemoveScanClick(e);
        }
    }
}
