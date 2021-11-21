using System;

namespace Catalog.Scrapers.MobyGames.Model
{
    public record ImageEntry
    {
        public ImageEntry(byte[] data, Uri url)
        {
            Data = data;
            Url = url;
        }

        public byte[] Data { get; set; }
        public Uri Url { get; set; }
    }
}
