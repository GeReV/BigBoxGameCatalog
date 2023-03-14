using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Catalog.Model
{
    public class GameCopy : IModel, ICloneable<GameCopy>
    {
        public GameCopy() : this(string.Empty)
        {
        }

        public GameCopy(string title)
        {
            Title = title;
        }

        public int GameCopyId { get; set; }

        [Required] public string Title { get; set; }

        public bool Sealed { get; set; }

        public string? Notes { get; set; }

        public string? MobyGamesSlug { get; set; }

        public uint? MobyGamesId { get; set; }

        public int PublisherId { get; set; }

        public Publisher? Publisher { get; set; }

        public List<GameCopyDeveloper> GameCopyDevelopers { get; set; } = new();

        public List<Developer> Developers => GameCopyDevelopers.Select(gcd => gcd.Developer).ToList();

        public ICollection<GameCopyTag> GameCopyTags { get; set; } = new List<GameCopyTag>();

        public IList<Tag> Tags => GameCopyTags
            .Select(gct => gct.Tag)
            .OrderBy(tag => tag.Name)
            .ToImmutableList();

        public DateTime ReleaseDate { get; set; }

        public List<string> TwoLetterIsoLanguageName { get; set; } = new();

        public List<Platform> Platforms { get; set; } = new();

        public List<string> Links { get; set; } = new();

        public string? CoverImage { get; set; }

        public List<string> Screenshots { get; set; } = new();

        public List<GameItem> Items { get; set; } = new();

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Id => GameCopyId;
        public bool IsNew => GameCopyId == 0;

        public GameCopy Clone() =>
            new(Title)
            {
                CoverImage = CoverImage,
                GameCopyDevelopers = GameCopyDevelopers
                    .Select(gcd => new GameCopyDeveloper { DeveloperId = gcd.DeveloperId })
                    .ToList(),
                GameCopyTags = GameCopyTags
                    .Select(gct => new GameCopyTag { TagId = gct.TagId })
                    .ToList(),
                Items = Items
                    .Select(item => item.Clone())
                    .ToList(),
                Links = Links.ToList(),
                MobyGamesSlug = MobyGamesSlug,
                MobyGamesId = MobyGamesId,
                Notes = Notes,
                Platforms = Platforms.ToList(),
                Publisher = Publisher,
                ReleaseDate = ReleaseDate,
                Screenshots = Screenshots,
                Sealed = Sealed,
                TwoLetterIsoLanguageName = TwoLetterIsoLanguageName
            };
    }
}
