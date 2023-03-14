using System.Collections.Generic;
using System.Linq;
using MobyGames.API.DataObjects;

namespace Catalog.Wpf.ViewModel
{
    public class GameDisambiguationViewModel
    {
        public class Item
        {
            public Item(string name, string releases, Game result)
            {
                Name = name;
                Releases = releases;
                Result = result;
            }

            public string Name { get; set; }
            public string Releases { get; set; }

            public Game Result { get; set; }
        }

        public IEnumerable<Item> Items { get; set; } = Enumerable.Empty<Item>();
    }
}
