using System.Collections.Generic;
using Catalog.Scrapers.MobyGames.Model;

namespace Catalog.Wpf.ViewModel
{
    public class CoverSelectionViewModel
    {
        public record Item
        {
            public Item(string platform, string? country, CoverArtEntry frontCover)
            {
                Platform = platform;
                Country = country;
                FrontCover = frontCover;
            }

            public string Platform { get; set; }
            public string? Country { get; set; }
            public CoverArtEntry FrontCover { get; set; }
        }

        public IEnumerable<Item> Items { get; set; } = new List<Item>();
    }
}
