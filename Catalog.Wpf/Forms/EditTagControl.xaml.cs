using System.Windows;
using System.Windows.Controls;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Forms
{
    public partial class EditTagControl : UserControl
    {
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(
            nameof(Tag), typeof(TagViewModel), typeof(EditTagControl), new PropertyMetadata(default(TagViewModel)));

        public TagViewModel Tag
        {
            get => (TagViewModel) GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        public EditTagControl()
        {
            InitializeComponent();
        }
    }
}
