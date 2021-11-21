using System;

namespace Catalog.Scrapers.MobyGames.Model
{
    public record CoverArtCollectionEntry
    {
        public CoverArtCollectionEntry(string platform, string? country = null)
        {
            Platform = platform;
            Country = country;
        }

        public string Platform { get; set; }
        public string? Country { get; set; }
        public CoverArtEntry[] Covers { get; set; } = Array.Empty<CoverArtEntry>();
    }
}
