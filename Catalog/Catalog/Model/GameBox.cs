using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class GameBox
    {
        public int GameBoxId { get; set; }
        public List<Image> Scans { get; set; }
    }
}
