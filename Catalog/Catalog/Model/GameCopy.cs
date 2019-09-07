using System;
using System.Collections.Generic;

namespace Catalog.Model
{
    public class GameCopy
    {
        public int GameCopyId { get; set; }
        public string Title { get; set; }
        public List<Developer> Developers { get; set; }
        public Publisher Publisher { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<string> TwoLetterISOLanguageName { get; set; }
        public Platform Platform { get; set; }
        public List<string> Links { get; set; }
        public List<string> Screenshots { get; set; }
        public GameBox GameBox { get; set; }
        public List<Media> Media { get; set; }
        public List<Appendix> Appendices { get; set; }
    }
}
