using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catalog.Model
{
    public class Developer : IEquatable<Developer>
    {
        public int DeveloperId { get; set; }

        [Required]
        public string Slug { get; set; }
        [Required]
        public string Name { get; set; }
        public List<string> Links { get; set; } = new List<string>();

//        public List<GameCopy> Games { get; set; } = new List<GameCopy>();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        public List<GameCopyDeveloper> GameCopyDevelopers { get; set; }

        public bool Equals(Developer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DeveloperId != 0 && DeveloperId == other.DeveloperId || Slug == other.Slug;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Developer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DeveloperId * 397) ^ (Slug != null ? Slug.GetHashCode() : 0);
            }
        }
    }
}
