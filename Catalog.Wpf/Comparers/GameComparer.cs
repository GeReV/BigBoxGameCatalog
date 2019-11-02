using System;
using System.Collections;
using System.Collections.Generic;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Comparers
{
    public class GameComparer : IComparer<MainWindowViewModel.Game>, IComparer
    {
        public int Compare(MainWindowViewModel.Game x, MainWindowViewModel.Game y) =>
            string.Compare(x?.Title, y?.Title, StringComparison.InvariantCultureIgnoreCase);

        public int Compare(object x, object y) =>
            Compare((MainWindowViewModel.Game) x, (MainWindowViewModel.Game) y);
    }
}