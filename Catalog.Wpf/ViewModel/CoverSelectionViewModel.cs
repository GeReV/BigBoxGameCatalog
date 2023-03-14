using System.Collections.Generic;
using MobyGames.API.DataObjects;

namespace Catalog.Wpf.ViewModel
{
    public class CoverSelectionViewModel
    {
        public record Item
        {
            public Item(string? country, Cover frontCover)
            {
                Country = country;
                FrontCover = frontCover;
            }

            public string? Country { get; set; }
            public Cover FrontCover { get; set; }
        }

        public IEnumerable<Item> Items { get; set; } = new List<Item>();
    }
}
