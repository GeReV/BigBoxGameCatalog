using System;
using System.Threading.Tasks;
using System.Windows;
using Catalog.Wpf.Extensions;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class DeleteGameCommand : AsyncCommandBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public DeleteGameCommand(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        protected override bool CanExecuteImpl(object? parameter)
        {
            return parameter is GameViewModel;
        }

        protected override async Task Perform(object? parameter)
        {
            if (parameter is not GameViewModel game)
            {
                return;
            }

            var messageResult = MessageBox.Show(
                $"Are you sure you want to delete this copy of {game.Title}?",
                "Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation
            );

            if (messageResult != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                await using var database = Application.Current.Database();

                database.RemoveRange(game.GameCopy.GameCopyTags);
                database.RemoveRange(game.GameCopy.GameCopyDevelopers);
                database.RemoveRange(game.GameCopy.Items);
                database.Remove(game.GameCopy);

                await database.SaveChangesAsync();
            }
            catch (Exception e)
            {
                mainWindowViewModel.CurrentException = e;
                mainWindowViewModel.Status = MainWindowViewModel.ViewStatus.Error;
            }

            mainWindowViewModel.Games.Remove(game);
        }
    }
}
