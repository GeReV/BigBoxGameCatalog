using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Comparers
{
    public class GameItemGroupComparer : IComparer<GameItemGroupViewModel>, IComparer
    {
        public int Compare(GameItemGroupViewModel? x, GameItemGroupViewModel? y)
        {
            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (!x.ItemType.Equals(y.ItemType))
            {
                var all = ItemTypes.All.ToList();

                var indexX = all.IndexOf(x.ItemType);
                var indexY = all.IndexOf(y.ItemType);

                return indexX.CompareTo(indexY);
            }

            if (x.Missing != y.Missing)
            {
                return x.Missing.CompareTo(y.Missing);
            }

            return x.Count.CompareTo(y.Count);
        }

        public int Compare(object? x, object? y) =>
            Compare((GameItemGroupViewModel?) x, (GameItemGroupViewModel?) y);
    }
}
