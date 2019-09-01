using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames.Model
{
    public class SearchResult : NamedEntry
    {
        public string[] Releases { get; set; }
    }
}
