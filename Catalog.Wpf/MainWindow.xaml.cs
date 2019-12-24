using System;
using System.Windows;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();

            DataContext = ViewModel;

            InitializeComponent();
        }

        public MainWindowViewModel ViewModel { get; }

        private void AddGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var editGameDialog = new EditGameDialog
            {
                Owner = this
            };

            if (editGameDialog.ShowDialog() == true)
            {
                CommandExecutor.Execute(ViewModel.RefreshGames);
            }
        }

        private void GameGalleryView_OnGameDoubleClick(object sender, EventArgs e)
        {
            CommandExecutor.Execute(ViewModel.EditGameCommand, ViewModel.FilteredGames.CurrentItem);
        }
    }
}
