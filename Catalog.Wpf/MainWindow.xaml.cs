using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Catalog.Wpf
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();


        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            DataContext = Application.Current.Database().GetGamesCollection().FindAll();
        }

        private void AddGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            new AddGameDialog().ShowDialog();
        }
    }
}