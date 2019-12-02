using System;
using System.Windows;
using System.Windows.Input;
using Catalog.Wpf.ViewModel;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace Catalog.Wpf
{
    public partial class EditGameDialog : Window
    {
        public EditGameDialog(EditGameViewModel viewModel = null)
        {
            InitializeComponent();

            ViewModel = viewModel ?? new EditGameViewModel(Application.Current.Database());

            Title = ViewModel.Game.IsNew ? "Add Game" : $"Edit Game: {ViewModel.GameTitle}";
        }

        public EditGameViewModel ViewModel
        {
            get => (EditGameViewModel) DataContext;
            set => DataContext = value;
        }

        private void EditGameDialog_OnContentRendered(object sender, EventArgs e)
        {
            TitleTextbox.Focus();
        }

        private async void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.SaveGameCommand.CanExecute(null))
            {
                return;
            }

            await ViewModel.SaveGameCommand.ExecuteAsync(null);

            DialogResult = true;

            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void AddItemMenu_OnClick(object sender, RoutedEventArgs e)
        {
            AddItemButton.IsOpen = false;
        }

        private void Screenshots_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                CommandExecutor.Execute(ViewModel.RemoveScreenshotCommand, Screenshots.SelectedItems);
            }
        }
    }
}
