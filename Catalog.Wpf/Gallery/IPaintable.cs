using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public interface IPaintable
    {
        SKSize DesiredSize { get; }
        void Measure(SKSize constraint);
        void Paint(SKCanvas canvas, SKPoint point);
    }
}
