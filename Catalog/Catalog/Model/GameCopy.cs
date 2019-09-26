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
            Screenshots = new List<Image>();
            Items = new List<Item>();
            TwoLetterIsoLanguageName = new List<string> { "en" };
        }

        public int GameCopyId { get; set; }
        public string Title { get; set; }

        public string Notes { get; set; }

        public string MobyGamesSlug { get; set; }
        public List<Developer> Developers { get; set; }
        public Publisher Publisher { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<string> TwoLetterIsoLanguageName { get; set; }
        public Platform Platform { get; set; }
        public List<string> Links { get; set; }
        public List<Image> Screenshots { get; set; }
        public List<Item> Items { get; set; }
    }
}
