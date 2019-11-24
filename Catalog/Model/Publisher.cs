using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catalog.Model
{
    public class Publisher : IEquatable<Publisher>
    {
        public int PublisherId { get; set; }
        [Required] public string Slug { get; set; }
        [Required] public string Name { get; set; }
        public List<string> Links { get; set; } = new List<string>();

        public List<GameCopy> Games { get; set; } = new List<GameCopy>();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Publisher other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PublisherId == other.PublisherId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Publisher) obj);
        }

        public override int GetHashCode() => PublisherId;
    }
}
