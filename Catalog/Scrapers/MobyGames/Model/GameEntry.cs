using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Scrapers.MobyGames.Model
{
    public class GameEntry : NamedEntry
    {
        public PublisherEntry? Publisher { get; set; }
        public DeveloperEntry[] Developers { get; set; } = Array.Empty<DeveloperEntry>();
        public string? ReleaseDate { get; set; }
        public string[] Platforms { get; set; } = Array.Empty<string>();
    }
}
