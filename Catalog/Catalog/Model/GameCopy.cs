using System;
using System.Collections.Generic;

namespace Catalog.Model
{
    public class GameCopy
    {
        public GameCopy()
        {
            Developers = new List<Developer>();
            Links = new List<string>();
            Screenshots = new List<string>();
            Media = new List<Media>();
            Appendices = new List<Appendix>();
            TwoLetterIsoLanguageName = new List<string>();
        }
        
        public int GameCopyId { get; set; }
        public string Title { get; set; }
        public List<Developer> Developers { get; set; }
        public Publisher Publisher { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<string> TwoLetterIsoLanguageName { get; set; }
        public Platform Platform { get; set; }
        public List<string> Links { get; set; }
        public List<string> Screenshots { get; set; }
        public GameBox GameBox { get; set; }
        public List<Media> Media { get; set; }
        public List<Appendix> Appendices { get; set; }
    }
}
