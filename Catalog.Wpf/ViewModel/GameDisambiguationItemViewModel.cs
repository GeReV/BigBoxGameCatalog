using System.Collections.Generic;
using Catalog.Scrapers.MobyGames.Model;

namespace Catalog.Wpf.ViewModel
{
    public class GameDisambiguationViewModel
    {
        public class Item
        {
            public Item(string name, string releases, SearchResult result)
            {
                Name = name;
                Releases = releases;
                Result = result;
            }

            public string Name { get; set; }
            public string Releases { get; set; }

            public SearchResult Result { get; set; }
        }

        public IEnumerable<Item> Items { get; set; } = new List<Item>();
    }
}
