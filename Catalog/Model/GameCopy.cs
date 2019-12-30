using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace Catalog.Model
{
    public class GameCopy : IModel, ICloneable<GameCopy>
    {
        public int GameCopyId { get; set; }

        [Required] public string Title { get; set; }

        public bool Sealed { get; set; }

        public string Notes { get; set; }

        public string MobyGamesSlug { get; set; }

        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public ICollection<GameCopyDeveloper> GameCopyDevelopers { get; set; }

        public IEnumerable<Developer> Developers => GameCopyDevelopers.Select(gcd => gcd.Developer);

        public ICollection<GameCopyTag> GameCopyTags { get; set; }

        public IList<Tag> Tags => GameCopyTags
            .Select(gct => gct.Tag)
            .OrderBy(tag => tag.Name)
            .ToImmutableList();

        public DateTime ReleaseDate { get; set; }

        public List<string> TwoLetterIsoLanguageName { get; set; } = new List<string>();

        public List<Platform> Platforms { get; set; } = new List<Platform>();

        public List<string> Links { get; set; } = new List<string>();

        public string CoverImage { get; set; }

        public List<string> Screenshots { get; set; } = new List<string>();

        public ICollection<GameItem> Items { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Id => GameCopyId;
        public bool IsNew => GameCopyId == 0;

        public GameCopy Clone() =>
            new GameCopy
            {
                CoverImage = CoverImage,
                GameCopyDevelopers = GameCopyDevelopers
                    .Select(gcd => new GameCopyDeveloper {DeveloperId = gcd.DeveloperId})
                    .ToList(),
                GameCopyTags = GameCopyTags
                    .Select(gct => new GameCopyTag {TagId = gct.TagId})
                    .ToList(),
                Items = Items
                    .Select(item => item.Clone())
                    .ToList(),
                Links = Links.ToList(),
                MobyGamesSlug = MobyGamesSlug,
                Notes = Notes,
                Platforms = Platforms.ToList(),
                Publisher = Publisher,
                ReleaseDate = ReleaseDate,
                Screenshots = Screenshots,
                Sealed = Sealed,
                Title = Title,
                TwoLetterIsoLanguageName = TwoLetterIsoLanguageName
            };
    }
}
