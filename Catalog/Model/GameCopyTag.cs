using System;

namespace Catalog.Model
{
    public class GameCopyTag : ITimestamps
    {
        public int GameCopyId { get; set; }
        public GameCopy Game { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
