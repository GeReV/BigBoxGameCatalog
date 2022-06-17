using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public sealed class WrappingLayout : Layout
    {
        public float HorizontalSpacing { get; init; }
        public float VerticalSpacing { get; init; }

        public WrappingLayout(IEnumerable<IPaintable> items) : base(items)
        {
        }

        public override void Paint(SKCanvas canvas, SKPoint point)
        {
            var cursor = SKPoint.Empty;
            var maxRowHeight = 0.0f;

            foreach (var item in Items)
            {
                var itemSize = item.DesiredSize;

                if (cursor.X + itemSize.Width > DesiredSize.Width)
                {
                    cursor.X = 0;
                    cursor.Y += maxRowHeight + VerticalSpacing;
                    
                    maxRowHeight = itemSize.Height;
                }
                
                maxRowHeight = Math.Max(maxRowHeight, itemSize.Height);

                if (cursor.Y >= DesiredSize.Height)
                {
                    // Can't render any more lines.
                    break;
                }
                
                item.Paint(canvas, point + cursor);
                
                cursor.X += itemSize.Width + HorizontalSpacing;
            }
        }

        public override void Measure(SKSize constraint)
        {
            var result = SKSize.Empty;
            var cursor = SKPoint.Empty;
            var maxRowHeight = 0.0f;

            DesiredSize = constraint;
            
            foreach (var item in Items)
            {
                item.Measure(constraint);

                var itemSize = item.DesiredSize;

                if (cursor.X + itemSize.Width > DesiredSize.Width)
                {
                    cursor.X = 0;
                    cursor.Y += maxRowHeight + VerticalSpacing;

                    result.Height = cursor.Y;
                    
                    maxRowHeight = itemSize.Height;
                }
                
                maxRowHeight = Math.Max(maxRowHeight, itemSize.Height);
                
                cursor.X += itemSize.Width + HorizontalSpacing;
                
                result.Width = Math.Max(result.Width, cursor.X - HorizontalSpacing);

                if (cursor.Y >= DesiredSize.Height)
                {
                    break;
                }
            }

            result.Height = Math.Clamp(result.Height + maxRowHeight, 0f, DesiredSize.Height);

            DesiredSize = result;
        }
    }
}
