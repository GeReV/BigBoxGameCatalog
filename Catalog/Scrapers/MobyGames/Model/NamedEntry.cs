using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames.Model
{
    public abstract class NamedEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
