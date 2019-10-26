using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class File : LocalResource
    {
        public byte[] Sha256Checksum { get; set; }
    }
}
