using System.Windows;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public partial class EditTagWindow : Window
    {
        public TagViewModel ViewModel
        {
            get => (TagViewModel) DataContext;
            set => DataContext = value;
        }

        public Tag ResultTag { get; private set; } = null;

        public EditTagWindow(Tag tag = null)
        {
            ViewModel = new TagViewModel(tag);

            InitializeComponent();

            Title = ViewModel.Tag.IsNew ? "Add Tag" : $"Edit Tag: {ViewModel.Title}";
        }

        private void ColorPicker_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ColorPickerIsOpen = !ViewModel.ColorPickerIsOpen;
        }

        private void EditLabelWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ColorPickerIsOpen = false;
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            ResultTag = ViewModel.Tag;

            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}
