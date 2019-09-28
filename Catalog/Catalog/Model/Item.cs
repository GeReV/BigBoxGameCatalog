using System.Collections.Generic;

namespace Catalog.Model
{
    public class Item
    {
        public ItemType ItemType { get; set; }
        public bool Missing { get; set; }
        public Condition? Condition { get; set; }
        public string ConditionDetails { get; set; }
        public List<Image> Scans { get; set; }
        public List<File> Files { get; set; }
        public string Notes { get; set; }
    }
}