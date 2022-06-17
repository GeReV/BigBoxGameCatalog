using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Catalog.Wpf.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Style = Topten.RichTextKit.Style;

namespace Catalog.Wpf.Gallery
{
    public sealed class GalleryItem : ElementBase, IBox
    {
        private const float LINE_MARGIN = 4f;
        private const float FONT_SIZE = 12f;
        private const float CORNER_RADIUS = 2.0f;

        private readonly Layout layout;
        public Thickness Padding { get; init; }
        public bool IsHighlighted { get; set; }
        public bool IsSelected { get; set; }
        public SKColor BorderColor { get; set; } = new(0xff70c0e7);
        public SKColor HighlightBackgroundColor { get; set; } = new(0xffe5f3fb);
        public SKColor SelectedBackgroundColor { get; set; } = new(0xffcbe8f6);

        public GalleryItem(GameViewModel game, SkiaTextureAtlas textureAtlas)
        {
            var textStyle = new Style
            {
                FontFamily = "system",
                FontSize = FONT_SIZE,
            };

            layout = new StackingLayout(
                Orientation.Vertical,
                new IPaintable[]
                {
                    new Image(textureAtlas, game.CoverPath),
                    new TextLine(game.Title ?? string.Empty, textStyle),
                    new WrappingLayout(game.Tags.Select(t => new Tag(t.Name, t.Color.ToSKColor())))
                    {
                        HorizontalSpacing = 4.0f,
                        VerticalSpacing = 2.0f
                    },
                }
            )
            {
                Spacing = LINE_MARGIN
            };
        }

        public override void Measure(SKSize constraint)
        {
            var contentConstraint = constraint;
            contentConstraint.Width -= (float)(Padding.Left + Padding.Right);
            contentConstraint.Height -= (float)(Padding.Top + Padding.Bottom);

            layout.Measure(contentConstraint);

            var contentSize = layout.DesiredSize;

            DesiredSize = new SKSize(
                constraint.Width,
                (float)Math.Ceiling(contentSize.Height + Padding.Top + Padding.Bottom)
            );
        }

        public override void Paint(SKCanvas canvas, SKPoint point)
        {
            DrawBackground(canvas, point);

            layout.Paint(canvas, point + new SKPoint((float)Padding.Left, (float)Padding.Top));
        }

        private void DrawBackground(SKCanvas canvas, SKPoint position)
        {
            SKColor backgroundColor;

            using var paint = new SKPaint
            {
                IsAntialias = true
            };

            if (IsSelected)
            {
                backgroundColor = SelectedBackgroundColor;
            }
            else if (IsHighlighted)
            {
                backgroundColor = HighlightBackgroundColor;
            }
            else
            {
                return;
            }

            var rect = SKRect.Create(position, ActualSize);

            paint.Color = backgroundColor;
            paint.Style = SKPaintStyle.Fill;

            canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);

            paint.Color = BorderColor;
            paint.Style = SKPaintStyle.Stroke;

            canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);
        }
    }
}
