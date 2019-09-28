using System;
using System.Windows;

namespace Catalog.Wpf
{
    public partial class AddGameDialog : Window
    {
        public AddGameDialog()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var database = Application.Current.Database();

            PublishersList.ItemsSource = database.GetPublishersCollection().FindAll();

            DevelopersCheckList.ItemsSource = database.GetDevelopersCollection().FindAll();
        }
    }
}