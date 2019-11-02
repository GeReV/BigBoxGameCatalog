using System.Collections.Generic;

namespace Catalog.Scrapers.MobyGames.Model
{
    public class Specs
    {
        public IEnumerable<string> Platforms { get; set; }
        public IEnumerable<string> MediaTypes { get; set; }
    }
}