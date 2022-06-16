using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public abstract class Layout : IPaintable
    {
        private readonly List<IPaintable> items;

        public SKSize DesiredSize { get; protected set; }

        public Layout() : this(Enumerable.Empty<IPaintable>())
        {
        }
        
        public Layout(IEnumerable<IPaintable> items)
        {
            this.items = new List<IPaintable>(items);
        }

        public void AddItem(IPaintable item) => items.Add(item);

        public IEnumerable<IPaintable> Items => items.AsReadOnly();
        
        public abstract void Measure(SKSize constraint);

        public abstract void Paint(SKCanvas canvas, SKPoint point);
    }
}
