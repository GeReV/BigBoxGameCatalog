using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Catalog.Wpf.ViewModel;
using MobyGames.API.DataObjects;

namespace Catalog.Wpf
{
    public partial class GameDisambiguationDialog : Window
    {
        private readonly ICollection<Game> entries;

        public GameDisambiguationDialog(ICollection<Game> entries)
        {
            InitializeComponent();

            this.entries = entries;

            DataContext = new GameDisambiguationViewModel
            {
                Items = entries.Select(
                    item => new GameDisambiguationViewModel.Item(
                        item.Title,
                        string.Join(", ", item.Platforms.Select(r => r.Name)),
                        item
                    )
                )
            };

            ResultCount.Text = $"Found {entries.Count} results:";

            ResultList.Focus();
        }

        public Game SelectedResult => ((GameDisambiguationViewModel.Item)ResultList.SelectedItem).Result;

        private void Close(bool result)
        {
            DialogResult = result;

            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Close(false);
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Close(true);
        }
    }
}
