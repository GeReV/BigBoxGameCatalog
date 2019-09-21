using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class Media
    {
        public MediaType Type { get; set; }
        public Image Scan { get; set; }
        public File File { get; set; }
    }
}
