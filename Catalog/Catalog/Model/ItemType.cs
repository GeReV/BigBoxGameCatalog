using System;

namespace Catalog.Model
{
    public class ItemType : IEquatable<ItemType>
    {
        public ItemType()
        {
        }

        public ItemType(string type, string description)
        {
            Type = type;
            Description = description;
        }

        public string Type { get; private set; }
        public string Description { get; private set; }

        public bool Equals(ItemType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemType) obj);
        }

        public override int GetHashCode()
        {
            return (Type != null ? Type.GetHashCode() : 0);
        }
    }
}