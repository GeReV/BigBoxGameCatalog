using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames.Model
{
    public class SearchResult : NamedEntry
    {
        public class Release
        {
            public string Text { get; set; }

            public string Platform { get; set; }
            public string Url { get; set; }
        }

        public Release[] Releases { get; set; }
    }
}
