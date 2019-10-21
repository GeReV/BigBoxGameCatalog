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

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        private void AddGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (new EditGameDialog().ShowDialog() == true && ViewModel.RefreshGames.CanExecute(null))
            {
                ViewModel.RefreshGames.Execute(null);
            }
        }

        private void Games_OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.EditGameCommand.CanExecute(Games.SelectedItem))
            {
                ViewModel.EditGameCommand.Execute(Games.SelectedItem);
            }
        }
    }
}