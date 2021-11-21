using System;

namespace Catalog.Model
{
    public class File : ILocalResource, IModel
    {
        public File() : this(string.Empty)
        {
        }

        public File(string path)
        {
            Path = path;
        }

        public int FileId { get; set; }

       public GameItem GameItem { get; set; }
        public byte[] Sha256Checksum { get; set; } = Array.Empty<byte>();
        public string Path { get; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Id => FileId;
        public bool IsNew => FileId == 0;
    }
}
