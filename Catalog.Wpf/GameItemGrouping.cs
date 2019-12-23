using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Model;
using Catalog.Wpf.Comparers;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf
{
    public static class GameItemGrouping
    {
        private static readonly GameItemGroupComparer GameItemGroupComparer = new GameItemGroupComparer();

        public static IEnumerable<GameItemGroupViewModel> GroupItems(IEnumerable<GameItem> items)
        {
            var result = items
                .Aggregate(
                    new Dictionary<Tuple<ItemType, bool>, GameItemGroupViewModel>(),
                    (groups, item) =>
                    {
                        var key = new Tuple<ItemType, bool>(item.ItemType, item.Missing);

                        if (groups.ContainsKey(key))
                        {
                            groups[key].Count += 1;
                        }
                        else
                        {
                            groups.Add(key, new GameItemGroupViewModel(item.ItemType)
                            {
                                Missing = item.Missing,
                                Count = 1
                            });
                        }

                        return groups;
                    }
                )
                .Values
                .ToList();

            result.Sort(GameItemGroupComparer);

            return result;
        }
    }
}
