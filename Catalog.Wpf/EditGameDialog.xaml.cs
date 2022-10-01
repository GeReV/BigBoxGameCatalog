using System;
using System.Windows;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace Catalog.Wpf
{
    public partial class EditGameDialog : Window
    {
        public static readonly RoutedUICommand DuplicateItemCommand = new RoutedUICommand(
            "Duplicate Item",
            "Duplicate",
            typeof(EditGameDialog),
            new InputGestureCollection
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            }
        );

        public EditGameDialog(int? gameCopyId = null)
        {
            InitializeComponent();

            var database = Application.Current.Database();

            ViewModel = new EditGameViewModel(
                this,
                gameCopyId.HasValue ? GamesRepository.LoadGame(database, gameCopyId.Value) : new GameCopy()
            );

            Title = ViewModel.IsNew ? "Add Game" : $"Edit Game: {ViewModel.Title}";
        }

        public EditGameViewModel ViewModel
        {
            get => (EditGameViewModel)DataContext;
            set => DataContext = value;
        }

        private void EditGameDialog_OnContentRendered(object sender, EventArgs e)
        {
            TitleTextbox.Focus();
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

        private void DuplicateGameItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommandExecutor.Execute(ViewModel.DuplicateItemCommand, ItemList.SelectedItem);
        }

        private void DuplicateGameItem_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.DuplicateItemCommand.CanExecute(ItemList.SelectedItem);
        }

        private void DeleteGameItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommandExecutor.Execute(ViewModel.RemoveItemCommand, ItemList.SelectedItem);
        }

        private void DeleteGameItem_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.RemoveItemCommand.CanExecute(ItemList.SelectedItem);
        }

        private void DeleteScreenshot_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommandExecutor.Execute(ViewModel.RemoveScreenshotCommand, Screenshots.SelectedItems);
        }

        private void DeleteScreenshot_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.RemoveScreenshotCommand.CanExecute(Screenshots.SelectedItems);
        }
    }
}
