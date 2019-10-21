using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class Image
    {
        public Image()
        {
        }

        public Image(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
