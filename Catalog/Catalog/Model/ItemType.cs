using Eto.Drawing;

namespace Catalog.Model
{
    public class ItemType
    {
        public ItemType(string type, string description, Eto.Drawing.Image icon)
        {
            Type = type;
            Description = description;
            Icon = icon;
        }

        public Item CreateItem() => new Item
        {
            ItemType = this
        };

        public string Type { get; }
        public string Description { get; }
        public Eto.Drawing.Image Icon { get; }
    }
}