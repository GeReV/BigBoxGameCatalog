using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Model
{
    public sealed class Publisher : IEquatable<Publisher>, IModel
    {
        public Publisher(string name, string slug)
        {
            Slug = slug;
            Name = name;
        }

        public int PublisherId { get; set; }
        [Required] public string Slug { get; set; }
        [Required] public string Name { get; set; }
        public List<string> Links { get; set; } = new();

        public ICollection<GameCopy> Games { get; set; } = new List<GameCopy>();

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Id => PublisherId;
        public bool IsNew => PublisherId == 0;

        public override string ToString() => Name;

        public bool Equals(Publisher? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PublisherId == other.PublisherId || Slug == other.Slug;
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is Publisher other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(PublisherId, Slug);
    }
}
