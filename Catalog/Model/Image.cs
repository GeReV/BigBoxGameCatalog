using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class Image : LocalResource
    {
        public Image()
        {
        }

        public Image(string path)
        {
            Path = path;
        }
    }
}