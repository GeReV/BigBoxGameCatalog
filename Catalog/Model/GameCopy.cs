using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Model
{
    public class GameCopy
    {
        public int GameCopyId { get; set; }

        [Required]
        public string Title { get; set; }

        public bool Sealed { get; set; }

        public string Notes { get; set; }

        public string MobyGamesSlug { get; set; }

        public List<Developer> Developers { get; set; } = new List<Developer>();

        public List<GameCopyDeveloper> GameCopyDevelopers { get; set; }

        public Publisher Publisher { get; set; }

        public DateTime ReleaseDate { get; set; }

        public List<string> TwoLetterIsoLanguageName { get; set; } = new List<string>();

        public List<Platform> Platforms { get; set; } = new List<Platform>();

        public List<string> Links { get; set; } = new List<string>();

        public Image CoverImage { get; set; }

        public List<Image> Screenshots { get; set; } = new List<Image>();

        public List<GameItem> Items { get; set; } = new List<GameItem>();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }
    }
}
