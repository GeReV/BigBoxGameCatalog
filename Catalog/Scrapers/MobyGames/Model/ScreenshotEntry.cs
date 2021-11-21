namespace Catalog.Scrapers.MobyGames.Model
{
    public record ScreenshotEntry
    {
        public ScreenshotEntry(string url, string thumbnail)
        {
            Url = url;
            Thumbnail = thumbnail;
        }

        public string Url { get; set; }
        public string Thumbnail { get; set; }
    }
}
