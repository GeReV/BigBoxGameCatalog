using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class Publisher : IEquatable<Publisher>
    {
        public int PublisherId { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string[] Links { get; set; }

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
