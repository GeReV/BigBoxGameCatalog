using System.Collections.Generic;

namespace Catalog.Model
{
    public class Item
    {
        public Item(ItemType itemType)
        {
            ItemType = itemType;
        }

        public ItemType ItemType { get; }
        public bool Missing { get; set; }
        public Condition? Condition { get; set; }
        public string ConditionDetails { get; set; }
        public List<Image> Scans { get; set; }
        public List<File> Files { get; set; }
        public string Notes { get; set; }
    }
}