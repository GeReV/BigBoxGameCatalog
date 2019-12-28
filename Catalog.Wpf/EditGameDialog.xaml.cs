using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace Catalog.Wpf
{
    public partial class EditGameDialog : Window
    {
        private static GameCopy? LoadGame(int gameCopyId)
        {
            using var db = Application.Current.Database();

            var game = db.Games.Find(gameCopyId);

            if (game == null)
            {
                // TODO: Create an exception for this.
                throw new Exception($"Game with ID {gameCopyId} not found.");
            }

            var entry = db.Entry(game);

            entry
                .Collection(v => v.Items)
                .Query()
                .Include(item => item.Files)
                .Include(item => item.Scans)
                .Load();

            entry
                .Collection(v => v.GameCopyDevelopers)
                .Query()
                .Include(gcd => gcd.Developer)
                .Load();

            entry
                .Reference(v => v.Publisher)
                .Load();

            return game;
        }

        public EditGameDialog(int? gameCopyId = null)
        {
            InitializeComponent();

            ViewModel = new EditGameViewModel(this, gameCopyId.HasValue ? LoadGame(gameCopyId.Value) : new GameCopy());

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
