using System.Collections.Generic;
using System.Linq;

namespace Catalog.Wpf.Gallery
{
    public abstract class Layout : ElementBase
    {
        private readonly List<IPaintable> items;

        protected Layout() : this(Enumerable.Empty<IPaintable>())
        {
        }

        protected Layout(IEnumerable<IPaintable> items)
        {
            this.items = new List<IPaintable>(items);
        }

        public void AddItem(IPaintable item) => items.Add(item);

        public IEnumerable<IPaintable> Items => items.AsReadOnly();
    }
}
