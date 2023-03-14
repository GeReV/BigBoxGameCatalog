using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Catalog.Model
{
    public sealed class Developer : IEquatable<Developer>, IModel
    {
        public Developer(string name, uint mobyGamesCompanyId)
        {
            Name = name;
            MobyGamesCompanyId = mobyGamesCompanyId;
        }

        public int DeveloperId { get; set; }

        [Required] public uint MobyGamesCompanyId { get; set; }

        [Required] public string Name { get; set; }

        public List<string> Links { get; set; } = new();

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public List<GameCopyDeveloper> GameCopyDevelopers { get; set; } = new();

        public List<GameCopy> Games => GameCopyDevelopers.Select(gcd => gcd.Game).ToList();

        public int Id => DeveloperId;
        public bool IsNew => DeveloperId == 0;

        public bool Equals(Developer? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DeveloperId == other.DeveloperId || MobyGamesCompanyId == other.MobyGamesCompanyId;
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is Developer other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(DeveloperId, MobyGamesCompanyId);
    }
}
