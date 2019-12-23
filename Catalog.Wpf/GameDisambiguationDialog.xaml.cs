using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Catalog.Scrapers.MobyGames.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public partial class GameDisambiguationDialog : Window
    {
        private readonly ICollection<SearchResult> entries;

        public GameDisambiguationDialog(ICollection<SearchResult> entries)
        {
            InitializeComponent();

            this.entries = entries;

            DataContext = new GameDisambiguationViewModel
            {
                Items = entries.Select(item => new GameDisambiguationViewModel.Item(item.Name, string.Join(", ", item.Releases), item))
            };

            ResultCount.Text = $"Found {entries.Count} results:";

            ResultList.Focus();
        }

        public SearchResult SelectedResult => ((GameDisambiguationViewModel.Item)ResultList.SelectedItem).Result;

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
