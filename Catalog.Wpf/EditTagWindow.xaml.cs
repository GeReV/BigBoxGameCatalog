using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public partial class EditTagWindow : Window
    {
        public TagViewModel ViewModel
        {
            get => (TagViewModel)DataContext;
            set => DataContext = value;
        }

        public Tag? ResultTag { get; private set; } = null;

        public EditTagWindow(Tag? tag = null)
        {
            ViewModel = new TagViewModel(tag ?? new Tag());

            InitializeComponent();

            Title = ViewModel.Tag.IsNew ? "Add Tag" : $"Edit Tag: {ViewModel.Name}";
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ValidateModel();

            if (ViewModel.HasErrors)
            {
                return;
            }

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
