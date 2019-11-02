namespace Catalog.Scrapers.MobyGames.Model
{
    public class CoverArtEntry
    {
        public enum CoverArtType
        {
            Front = 0,
            Back,
            Media,
            Other
        }

        public CoverArtType Type { get; set; }

        public string Url { get; set; }

        public string Thumbnail { get; set; }
    }
}