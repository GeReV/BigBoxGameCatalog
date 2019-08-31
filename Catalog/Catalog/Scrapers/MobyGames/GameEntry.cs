using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames
{
    public class GameEntry
    {
        public string Name { get; set; }
        public string Href { get; set; }
        public string[] Releases { get; set; }
    }
}
