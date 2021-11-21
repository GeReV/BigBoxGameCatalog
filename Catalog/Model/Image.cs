using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catalog.Model
{
    public sealed class Image : ILocalResource, IModel
    {
        public Image(string path)
        {
            Path = path;
        }

        public int ImageId { get; set; }

        public GameItem GameItem { get; set; }
        public string Path { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Id => ImageId;
        public bool IsNew => ImageId == 0;
    }
}
