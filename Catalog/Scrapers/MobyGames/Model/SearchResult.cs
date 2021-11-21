using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames.Model
{
    public class SearchResult : NamedEntry
    {
        public record Release
        {
            public Release(string text, string platform, string url)
            {
                Text = text;
                Platform = platform;
                Url = url;
            }

            public string Text { get; set; }

            public string Platform { get; set; }
            public string Url { get; set; }
        }

        public Release[] Releases { get; set; } = Array.Empty<Release>();
    }
}
