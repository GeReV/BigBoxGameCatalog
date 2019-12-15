using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catalog.Model
{
    public class Image : ILocalResource, IModel
    {
        public int ImageId { get; set; }

        public virtual GameItem GameItem { get; set; }
        public string Path { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public Image()
        {
        }

        public Image(string path)
        {
            Path = path;
        }

        public bool Equals(ILocalResource other)
        {
            return Path == other.Path;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Image) obj);
        }

        public override int GetHashCode()
        {
            return (Path != null ? Path.GetHashCode() : 0);
        }

        public int Id => ImageId;
        public bool IsNew => ImageId == 0;
    }
}
