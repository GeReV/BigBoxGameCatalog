using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Forms
{
    class ListItemImageBinding : PropertyBinding<Image>
    {
        public ListItemImageBinding()
            : base("Image", true)
        {
        }

        protected override Image InternalGetValue(object dataItem)
        {
            if (dataItem is IImageListItem imageListItem)
                return imageListItem.Image;
			
            return base.InternalGetValue(dataItem);
        }
    }
}