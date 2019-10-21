using System.Collections.Generic;
using Catalog.Scrapers.MobyGames.Model;

namespace Catalog.Wpf.ViewModel
{
    public class CoverSelectionViewModel
    {
        public class Item
        {
            public string Platform { get; set; }
            public string Country { get; set; }
            public CoverArtEntry FrontCover { get; set; }
        }

        public IEnumerable<Item> Items { get; set; }
    }
}