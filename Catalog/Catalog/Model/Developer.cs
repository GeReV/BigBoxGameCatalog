using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class Developer
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string[] Links { get; set; }
    }
}
