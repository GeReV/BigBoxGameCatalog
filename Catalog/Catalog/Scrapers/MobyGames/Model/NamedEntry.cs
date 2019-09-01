using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames.Model
{
    public abstract class NamedEntry
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
    }
}
