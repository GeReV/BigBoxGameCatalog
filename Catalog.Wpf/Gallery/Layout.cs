using System.Collections.Generic;
using System.Linq;

namespace Catalog.Wpf.Gallery
{
    public abstract class Layout : BoxElement
    {
        private readonly List<ElementBase> items;

        public IEnumerable<IPaintable> Items => items.AsReadOnly();

        protected Layout() : this(Enumerable.Empty<ElementBase>())
        {
        }

        protected Layout(IEnumerable<ElementBase> items)
        {
            this.items = new List<ElementBase>(items);

            foreach (var item in this.items)
            {
                item.Parent = this;
            }
        }

        public void AddItem(ElementBase item)
        {
            item.Parent = this;

            items.Add(item);
        }
    }
}
