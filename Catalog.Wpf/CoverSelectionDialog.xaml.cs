using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Catalog.Scrapers.MobyGames.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public partial class CoverSelectionDialog : Window
    {
        public CoverSelectionDialog(ICollection<CoverArtCollectionEntry> entries)
        {
            InitializeComponent();

            DataContext = new CoverSelectionViewModel
            {
                Items = entries
                    .Where(entry => entry.Covers.Any(cover => cover.Type == CoverArtEntry.CoverArtType.Front))
                    .Select(entry =>
                    {
                        return new CoverSelectionViewModel.Item(
                            entry.Platform,
                            entry.Country,
                            entry.Covers.FirstOrDefault(cover => cover.Type == CoverArtEntry.CoverArtType.Front)!
                        );
                    })
            };

            ResultCount.Text = $"Found {entries.Count} covers:";

            ResultList.Focus();
        }

        public CoverArtEntry SelectedResult => ((CoverSelectionViewModel.Item) ResultList.SelectedItem).FrontCover;

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
