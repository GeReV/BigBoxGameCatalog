using Eto.Forms;

namespace Catalog.Forms
{
    class ListItemKeyBinding : PropertyBinding<string>
    {
        public ListItemKeyBinding()
            : base("Key", true)
        {
        }

        protected override string InternalGetValue(object dataItem)
        {
            if (dataItem is IListItem listItem)
            {
                return listItem.Key;
            }

            if (HasProperty(dataItem))
            {
                return base.InternalGetValue(dataItem);
            }

            return dataItem?.ToString();
        }
    }
}