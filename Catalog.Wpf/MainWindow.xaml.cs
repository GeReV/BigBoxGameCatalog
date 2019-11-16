using System;
using System.Windows;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        private void AddGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (new EditGameDialog().ShowDialog() == true)
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
