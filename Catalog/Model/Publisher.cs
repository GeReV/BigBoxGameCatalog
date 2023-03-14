using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Model
{
    public sealed class Publisher : IEquatable<Publisher>, IModel
    {
        public Publisher(string name, uint mobyGamesCompanyId)
        {
            Name = name;
            MobyGamesCompanyId = mobyGamesCompanyId;
        }

        public int PublisherId { get; set; }

        [Required] public uint MobyGamesCompanyId { get; set; }

        [Required] public string Name { get; set; }
        public List<string> Links { get; set; } = new();

        public List<GameCopy> Games { get; set; } = new();

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Id => PublisherId;
        public bool IsNew => PublisherId == 0;

        public override string ToString() => Name;

        public bool Equals(Publisher? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PublisherId == other.PublisherId || MobyGamesCompanyId == other.MobyGamesCompanyId;
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is Publisher other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(PublisherId, MobyGamesCompanyId);
    }
}
