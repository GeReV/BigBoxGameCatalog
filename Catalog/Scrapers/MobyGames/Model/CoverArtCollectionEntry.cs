namespace Catalog.Scrapers.MobyGames.Model
{
    public class CoverArtCollectionEntry
    {
        public string Platform { get; set; }
        public string Country { get; set; }
        public CoverArtEntry[] Covers { get; set; }
    }
}