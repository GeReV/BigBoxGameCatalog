using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catalog.Model
{
    public class File : ILocalResource, IModel
    {
        public int FileId { get; set; }

        public GameItem GameItem { get; set; }
        public byte[] Sha256Checksum { get; set; }
        public string Path { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool Equals(ILocalResource other)
        {
            return Path == other.Path;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((File) obj);
        }

        public override int GetHashCode()
        {
            return (Path != null ? Path.GetHashCode() : 0);
        }

        public int Id => FileId;
        public bool IsNew => FileId == 0;
    }
}
