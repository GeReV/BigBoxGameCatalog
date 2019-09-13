using Eto.Forms;

namespace Catalog.Forms
{
    class ListItemTextBinding : PropertyBinding<string>
    {
        public ListItemTextBinding()
            : base("Text", true)
        {
        }

        protected override string InternalGetValue(object dataItem)
        {
            IListItem listItem = dataItem as IListItem;
            if (listItem != null)
                return listItem.Text;
            if (this.HasProperty(dataItem))
                return base.InternalGetValue(dataItem);
            if (dataItem == null)
                return (string) null;
            return System.Convert.ToString(dataItem);
        }

        protected override void InternalSetValue(object dataItem, string value)
        {
            IListItem listItem = dataItem as IListItem;
            if (listItem != null)
                listItem.Text = System.Convert.ToString((object) value);
            else
                base.InternalSetValue(dataItem, value);
        }
    }
}