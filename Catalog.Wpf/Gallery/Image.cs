using System;
using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public sealed class Image : ElementBase
    {
        private readonly SkiaTextureAtlas atlas;
        private readonly string? imageKey;

        public float AspectRatio { get; init; } = 4f / 3f;

        public Image(SkiaTextureAtlas atlas, string? imageKey)
        {
            this.atlas = atlas;
            this.imageKey = imageKey;
        }

        public override void Measure(SKSize constraint)
        {
            DesiredSize = new SKSize(
                constraint.Width,
                (float)Math.Min(Math.Floor(constraint.Width * AspectRatio), constraint.Height)
            );
        }

        public override void Paint(SKCanvas canvas, SKPoint point)
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
                using var paint = new SKPaint
                {
                    Color = SKColors.Transparent,
                    Style = SKPaintStyle.Fill
                };

                atlas.DrawSprite(canvas, imageKey, SKRect.Create(point, DesiredSize), paint);
            }
        }
    }
}
