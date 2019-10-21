using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Catalog.Model
{
    public class GameCopy
    {
        public int GameCopyId { get; set; }

        public string Title { get; set; }

        public bool Sealed { get; set; }

        public string Notes { get; set; }

        public string MobyGamesSlug { get; set; }

        public List<Developer> Developers { get; set; }

        public Publisher Publisher { get; set; }

        public DateTime ReleaseDate { get; set; }

        public List<string> TwoLetterIsoLanguageName { get; set; }

        public List<Platform> Platforms { get; set; }

        public List<string> Links { get; set; }

        public Image CoverImage { get; set; }

        public List<Image> Screenshots { get; set; }

        public List<Item> Items { get; set; }
    }
}