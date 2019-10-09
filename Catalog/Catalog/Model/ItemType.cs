using Eto.Drawing;

namespace Catalog.Model
{
    public class ItemType
    {
        public ItemType(string type, string description)
        {
            Type = type;
            Description = description;
        }

        public string Type { get; }
        public string Description { get; }
    }
}