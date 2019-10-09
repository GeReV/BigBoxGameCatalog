using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Catalog.Model
{
    public class Item
    {
        public ItemType ItemType { get; set; }

        public bool Missing { get; set; }

        public Condition? Condition { get; set; }

        public string ConditionDetails { get; set; }

        public string Notes { get; set; }

        public IEnumerable<Image> Scans { get; set; }

        public IEnumerable<File> Files { get; set; }
    }
}