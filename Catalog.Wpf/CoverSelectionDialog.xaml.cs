using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Catalog.Wpf.ViewModel;
using MobyGames.API.DataObjects;

namespace Catalog.Wpf
{
    public partial class CoverSelectionDialog : Window
    {
        public CoverSelectionDialog(ICollection<CoverGroup> entries)
        {
            InitializeComponent();

            DataContext = new CoverSelectionViewModel
            {
                Items = entries
                    .Where(entry => entry.Covers.Any(cover => cover.ScanOf == CoverScanOf.FrontCover))
                    .Select(
                        entry => new CoverSelectionViewModel.Item(
                            string.Join(", ", entry.Countries),
                            entry.Covers.First(cover => cover.ScanOf == CoverScanOf.FrontCover)
                        )
                    )
            };

            ResultCount.Text = $"Found {entries.Count} covers:";

            ResultList.Focus();
        }

        public Cover SelectedResult => ((CoverSelectionViewModel.Item)ResultList.SelectedItem).FrontCover;

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
