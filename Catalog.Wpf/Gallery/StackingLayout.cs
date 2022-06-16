using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public sealed class StackingLayout : Layout
    {
        public float Spacing { get; set; }
        
        public Orientation Orientation { get; set; }

        public StackingLayout(Orientation orientation = Orientation.Horizontal) : this(orientation, Enumerable.Empty<IPaintable>())
        {
        }

        public StackingLayout(IEnumerable<IPaintable> items) : base(items)
        {
        }
        
        public StackingLayout(Orientation orientation, IEnumerable<IPaintable> items) : base(items)
        {
            Orientation = orientation;
        }

        public override void Paint(SKCanvas canvas, SKPoint point)
        {
            var cursor = SKPoint.Empty;
            var maxOrthogonalDimension = 0.0f;
            
            switch (Orientation)
            {
                case Orientation.Horizontal:
                {
                    foreach (var item in Items)
                    {
                        item.Measure(DesiredSize);

                        var itemSize = item.DesiredSize;

                        maxOrthogonalDimension = Math.Max(maxOrthogonalDimension, itemSize.Height);

                        if (cursor.X + itemSize.Width > DesiredSize.Width)
                        {
                            break;
                        }

                        item.Paint(canvas, point + cursor);
                
                        cursor.X += itemSize.Width + Spacing;
                    }

                    break;
                }
                case Orientation.Vertical:
                {
                    foreach (var item in Items)
                    {
                        item.Measure(DesiredSize);

                        var itemSize = item.DesiredSize;

                        maxOrthogonalDimension = Math.Max(maxOrthogonalDimension, itemSize.Width);

                        if (cursor.Y + itemSize.Height > DesiredSize.Height)
                        {
                            break;
                        }

                        item.Paint(canvas, point + cursor);
                
                        cursor.Y += itemSize.Height + Spacing;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(Orientation));
            }
        }

        public override void Measure(SKSize constraint)
        {
            var result = SKSize.Empty;
            var cursor = SKPoint.Empty;
            
            // The max row height if horizontal, the max column width if vertical.
            var maxOrthogonalDimension = 0.0f;

            DesiredSize = constraint;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                {
                    foreach (var item in Items)
                    {
                        item.Measure(constraint);

                        var itemSize = item.DesiredSize;

                        maxOrthogonalDimension = Math.Max(maxOrthogonalDimension, itemSize.Height);

                        if (cursor.X + itemSize.Width >= DesiredSize.Width)
                        {
                            break;
                        }

                        cursor.X += itemSize.Width + Spacing;

                        result.Width = Math.Max(result.Width, cursor.X - Spacing);
                    }
                    
                    result.Height = maxOrthogonalDimension;

                    break;
                }
                case Orientation.Vertical:
                {
                    foreach (var item in Items)
                    {
                        item.Measure(constraint);

                        var itemSize = item.DesiredSize;

                        maxOrthogonalDimension = Math.Max(maxOrthogonalDimension, itemSize.Width);

                        if (cursor.Y + itemSize.Height >= DesiredSize.Height)
                        {
                            break;
                        }

                        cursor.Y += itemSize.Height + Spacing;

                        result.Height = Math.Max(result.Height, cursor.Y - Spacing);
                    }

                    result.Width = maxOrthogonalDimension;

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(Orientation));
            }

            DesiredSize = result;
        }
    }
}
