using System;

namespace Catalog.Model
{
    public abstract class LocalResource : IEquatable<LocalResource>
    {
        public int LocalResourceId { get; set; }

        public string Path { get; set; }

        public bool Equals(LocalResource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Path == other.Path;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LocalResource) obj);
        }

        public override int GetHashCode()
        {
            return (Path != null ? Path.GetHashCode() : 0);
        }
    }
}
