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

        public EditTagWindow()
        {
            ViewModel = new TagViewModel();

            InitializeComponent();
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
            ResultTag = new Tag
            {
                Color = ViewModel.Color,
                Name = ViewModel.Title
            };

            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}
