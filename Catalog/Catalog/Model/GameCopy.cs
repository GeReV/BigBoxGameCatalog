using System;
using System.Collections.Generic;

namespace Catalog.Model
{
    public class GameCopy
    {
        public GameCopy()
        {
            GameBox = new GameBox();
            Developers = new List<Developer>();
            Links = new List<string>();
            Screenshots = new List<Image>();
            Media = new List<Media>();
            Appendices = new List<Appendix>();
            TwoLetterIsoLanguageName = new List<string> { "en" };
        }

        public int GameCopyId { get; set; }
        public string Title { get; set; }

        public string MobyGamesSlug { get; set; }
        public List<Developer> Developers { get; set; }
        public Publisher Publisher { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<string> TwoLetterIsoLanguageName { get; set; }
        public Platform Platform { get; set; }
        public List<string> Links { get; set; }
        public List<Image> Screenshots { get; set; }
        public GameBox GameBox { get; set; }
        public List<Media> Media { get; set; }
        public List<Appendix> Appendices { get; set; }
    }
}
