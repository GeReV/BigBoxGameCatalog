using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace Catalog.Model
{
    public class GameCopy : IModel
    {
        public int GameCopyId { get; set; }

        [Required] public string Title { get; set; }

        public bool Sealed { get; set; }

        public string Notes { get; set; }

        public string MobyGamesSlug { get; set; }

        public virtual Publisher Publisher { get; set; }

        public virtual ICollection<Developer> Developers { get; set; }

        public virtual ICollection<GameCopyDeveloper> GameCopyDevelopers { get; set; }

        public DateTime ReleaseDate { get; set; }

        public List<string> TwoLetterIsoLanguageName { get; set; } = new List<string>();

        public List<Platform> Platforms { get; set; } = new List<Platform>();

        public List<string> Links { get; set; } = new List<string>();

        public string CoverImage { get; set; }

        public List<string> Screenshots { get; set; } = new List<string>();

        public virtual ICollection<GameItem> Items { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsNew => GameCopyId == 0;
    }
}
