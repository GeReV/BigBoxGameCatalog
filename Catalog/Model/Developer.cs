using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Catalog.Model
{
    public sealed class Developer : IEquatable<Developer>, IModel
    {
        public Developer(string name, string slug)
        {
            Name = name;
            Slug = slug;
        }

        public int DeveloperId { get; set; }

        [Required] public string Slug { get; }

        [Required] public string Name { get; set; }
        public List<string> Links { get; set; } = new();


        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }


        public ICollection<GameCopyDeveloper> GameCopyDevelopers { get; set; } = new List<GameCopyDeveloper>();

        public IEnumerable<GameCopy> Games => GameCopyDevelopers.Select(gcd => gcd.Game);

        public int Id => DeveloperId;
        public bool IsNew => DeveloperId == 0;

        public bool Equals(Developer? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DeveloperId == other.DeveloperId || Slug == other.Slug;
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is Developer other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(DeveloperId, Slug);
    }
}
