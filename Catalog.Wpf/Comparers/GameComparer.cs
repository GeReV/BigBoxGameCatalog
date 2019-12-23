using System;
using System.Collections;
using System.Collections.Generic;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Comparers
{
    public class GameComparer : IComparer<GameViewModel>, IComparer
    {
        public int Compare(GameViewModel? x, GameViewModel? y) =>
            string.Compare(x?.Title, y?.Title, StringComparison.InvariantCultureIgnoreCase);

        public int Compare(object? x, object? y) =>
            Compare((GameViewModel?) x, (GameViewModel?) y);
    }
}
