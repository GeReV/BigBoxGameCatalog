﻿using System.Linq;
using System.Windows;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.Commands
{
    public class EditGameCommand : CommandBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public EditGameCommand(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is not int gameId)
            {
                return;
            }

            var editGameDialog = new EditGameDialog(gameId)
            {
                Owner = Application.Current.MainWindow
            };

            if (editGameDialog.ShowDialog() == true)
            {
                mainWindowViewModel.RefreshGame(gameId);
            }
        }
    }
}
