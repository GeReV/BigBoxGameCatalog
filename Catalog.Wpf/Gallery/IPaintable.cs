using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public interface IPaintable
    {
        SKSize DesiredSize { get; }
        SKSize RenderSize { get; }
        void Measure(SKSize constraint);
        void Arrange(SKSize finalSize);
        void InvalidateMeasure();
        void InvalidateArrange();
        void Paint(SKCanvas canvas, SKPoint point);
    }
}
