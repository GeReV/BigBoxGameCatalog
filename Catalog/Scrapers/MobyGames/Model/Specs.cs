using System.Collections.Generic;

namespace Catalog.Scrapers.MobyGames.Model
{
    public class Specs
    {
        public IEnumerable<string> Platforms { get; set; } = new List<string>();
        public IEnumerable<string> MediaTypes { get; set; } = new List<string>();
    }
}
