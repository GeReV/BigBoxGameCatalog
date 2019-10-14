using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
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

        private MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        private void AddGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (new AddGameDialog().ShowDialog() == true && ViewModel.RefreshGames.CanExecute(null))
            {
                ViewModel.RefreshGames.Execute(null);
            }
        }
    }
}