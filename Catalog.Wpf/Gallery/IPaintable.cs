using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public interface IPaintable
    {
        SKSize DesiredSize { get; }
        SKSize ActualSize { get; }
        void Measure(SKSize constraint);
        void Arrange(SKSize finalSize);
        void Paint(SKCanvas canvas, SKPoint point);
    }
}
