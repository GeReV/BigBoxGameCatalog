using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class GameBox
    {
        public int Id { get; set; }
        public List<Image> Scans { get; set; }
    }
}
