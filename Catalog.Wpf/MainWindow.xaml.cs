using System;
using System.Windows;
using System.Windows.Input;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "E_xit",
            "Exit",
            typeof(MainWindow),
            new InputGestureCollection
            {
                new KeyGesture(Key.Q, ModifierKeys.Control),
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );

        public static RoutedUICommand DuplicateGame = new RoutedUICommand(
            "_Duplicate Game",
            "Duplicate",
            typeof(MainWindow),
            new InputGestureCollection
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            }
        );

        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();

            DataContext = ViewModel;

            InitializeComponent();
        }

        public MainWindowViewModel ViewModel { get; }

        private void NewGame_Executed(object sender, RoutedEventArgs e)
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

        private void GameView_OnGameDoubleClick(object sender, EventArgs e)
        {
            var gameViewModel = (GameViewModel) ViewModel.FilteredGames.CurrentItem;

            CommandExecutor.Execute(ViewModel.EditGameCommand, gameViewModel.GameCopy.GameCopyId);
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DuplicateGame_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var gameViewModel = (GameViewModel) ViewModel.FilteredGames.CurrentItem;

            e.CanExecute = ViewModel.DuplicateGameCommand.CanExecute(gameViewModel.GameCopyId);
        }

        private void DuplicateGame_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var gameViewModel = (GameViewModel) ViewModel.FilteredGames.CurrentItem;

            CommandExecutor.Execute(ViewModel.DuplicateGameCommand, gameViewModel.GameCopyId);
        }

        private void DeleteGame_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.DeleteGameCommand.CanExecute(ViewModel.FilteredGames.CurrentItem);
        }

        private void DeleteGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommandExecutor.Execute(ViewModel.DeleteGameCommand, ViewModel.FilteredGames.CurrentItem);
        }
    }
}
