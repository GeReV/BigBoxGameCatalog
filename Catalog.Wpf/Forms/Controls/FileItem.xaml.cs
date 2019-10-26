using System.Windows;
using System.Windows.Controls;
using Catalog.Model;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class FileItem : UserControl
    {
        public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
            nameof(File), typeof(LocalResource), typeof(FileItem), new PropertyMetadata(default(LocalResource)));

        public LocalResource File
        {
            get => (LocalResource) GetValue(FileProperty);
            set => SetValue(FileProperty, value);
        }
        public FileItem()
        {
            InitializeComponent();
        }
    }
}