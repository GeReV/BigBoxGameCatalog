using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Forms.Controls;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Catalog.Wpf.ViewModel;
using Eto.Drawing;
using Eto.Forms;
using Xceed.Wpf.Toolkit;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Window = System.Windows.Window;

namespace Catalog.Wpf
{
    public partial class AddGameDialog : Window
    {
        public AddGameDialog()
        {
            InitializeComponent();

            ViewModel = new AddGameViewModel(Application.Current.Database());
        }

        public AddGameViewModel ViewModel
        {
            get => (AddGameViewModel) DataContext;
            set => DataContext = value;
        }

        private void AddGameDialog_OnContentRendered(object sender, EventArgs e)
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
    }
}