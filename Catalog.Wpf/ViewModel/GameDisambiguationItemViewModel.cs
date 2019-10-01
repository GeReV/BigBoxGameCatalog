using System.Collections.Generic;
using Catalog.Scrapers.MobyGames.Model;

namespace Catalog.Wpf.ViewModel
{
    public class GameDisambiguationViewModel
    {
        public class Item
        {
            public string Name { get; set; }
            public string Releases { get; set; }

            public SearchResult Result { get; set; }
        }

        public IEnumerable<Item> Items { get; set; }
    }
}