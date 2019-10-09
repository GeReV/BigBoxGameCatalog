using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class File
    {
        public string Path { get; set; }

        public byte[] Sha256Checksum { get; set; }
    }
}
