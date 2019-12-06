using System;

namespace Catalog.Model
{
    public class GameCopyTag : ITimestamps
    {
        public int GameCopyId { get; set; }
        public virtual GameCopy Game { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
