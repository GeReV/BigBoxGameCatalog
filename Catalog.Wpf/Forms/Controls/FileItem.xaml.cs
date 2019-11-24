using System.Windows;
using System.Windows.Controls;
using Catalog.Model;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class FileItem : UserControl
    {
        public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
            nameof(File), typeof(ILocalResource), typeof(FileItem), new PropertyMetadata(default(ILocalResource)));

        public ILocalResource File
        {
            get => (ILocalResource) GetValue(FileProperty);
            set => SetValue(FileProperty, value);
        }
        public FileItem()
        {
            InitializeComponent();
        }
    }
}