using System.Windows;
using System.Windows.Controls;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Forms
{
    public partial class EditTagForm : UserControl
    {
        public static readonly DependencyProperty GameTagProperty = DependencyProperty.Register(
            nameof(GameTag), typeof(TagViewModel), typeof(EditTagForm), new PropertyMetadata(default(TagViewModel)));

        public TagViewModel GameTag
        {
            get => (TagViewModel) GetValue(GameTagProperty);
            set => SetValue(GameTagProperty, value);
        }

        public EditTagForm()
        {
            InitializeComponent();
        }
    }
}
