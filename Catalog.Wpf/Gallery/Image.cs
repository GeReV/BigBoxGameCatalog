using System;
using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public class Image : IPaintable
    {
        private readonly SkiaTextureAtlas atlas;
        private readonly string? imageKey;
        public SKSize DesiredSize { get; private set; }

        public float AspectRatio { get; init; } = 4f / 3f;

        public Image(SkiaTextureAtlas atlas, string? imageKey)
        {
            this.atlas = atlas;
            this.imageKey = imageKey;
        }

        public void Measure(SKSize constraint)
        {
            DesiredSize = new SKSize(
                constraint.Width,
                (float)Math.Min(Math.Floor(constraint.Width * AspectRatio), constraint.Height)
            );
        }

        public void Paint(SKCanvas canvas, SKPoint point)
        {
            if (string.IsNullOrEmpty(imageKey))
            {
                using var paint = new SKPaint
                {
                    Color = SKColors.Gray,
                    Style = SKPaintStyle.Fill
                };

                canvas.DrawRect(SKRect.Create(point, DesiredSize), paint);
            }
            else
            {
                atlas.DrawSprite(canvas, imageKey, SKRect.Create(point, DesiredSize));
            }
        }
    }
}
