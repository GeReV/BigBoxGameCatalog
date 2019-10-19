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

        private MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        private void AddGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (new EditGameDialog().ShowDialog() == true && ViewModel.RefreshGames.CanExecute(null))
            {
                ViewModel.RefreshGames.Execute(null);
            }
        }
    }
}