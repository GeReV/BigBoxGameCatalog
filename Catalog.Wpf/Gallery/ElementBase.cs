using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public abstract class ElementBase : IPaintable
    {
        public SKSize DesiredSize { get; protected set; }
        
        public SKSize ActualSize { get; protected set; }

        public abstract void Measure(SKSize constraint);

        public virtual void Arrange(SKSize finalSize)
        {
            ActualSize = finalSize;
        }

        public abstract void Paint(SKCanvas canvas, SKPoint point);
    }
}
