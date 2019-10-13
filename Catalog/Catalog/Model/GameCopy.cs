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

        public string Notes { get; set; }

        public string MobyGamesSlug { get; set; }

        public IEnumerable<Developer> Developers { get; set; }

        public Publisher Publisher { get; set; }

        public DateTime ReleaseDate { get; set; }

        public IEnumerable<string> TwoLetterIsoLanguageName { get; set; }

        public IEnumerable<Platform> Platforms { get; set; }

        public IEnumerable<string> Links { get; set; }

        public IEnumerable<Image> Screenshots { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }
}