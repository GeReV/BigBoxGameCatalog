using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class Developer : IEquatable<Developer>
    {
        public int DeveloperId { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string[] Links { get; set; }

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
