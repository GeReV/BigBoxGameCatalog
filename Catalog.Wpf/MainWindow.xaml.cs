using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catalog.Model;
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
            if (new EditGameDialog().ShowDialog() == true && ViewModel.RefreshGames.CanExecute(null))
            {
                ViewModel.RefreshGames.Execute(null);
            }
        }

        private void Games_OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var gamesCollection = Application.Current.Database().GetGamesCollection();

            var gameCopy = gamesCollection
                .IncludeAll(1)
                .FindById(((GameCopy) Games.SelectedItem).GameCopyId);

            var viewModel = EditGameViewModel.FromGameCopy(gameCopy, Application.Current.Database());

            if (new EditGameDialog(viewModel).ShowDialog() == true && ViewModel.RefreshGames.CanExecute(null))
            {
                ViewModel.RefreshGames.Execute(null);
            }
        }
    }
}