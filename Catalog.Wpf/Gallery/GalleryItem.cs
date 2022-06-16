using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Catalog.Wpf.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Style = Topten.RichTextKit.Style;

namespace Catalog.Wpf.Gallery
{
    public sealed class GalleryItem : IBox
    {
        private const float LINE_MARGIN = 4f;
        private const float FONT_SIZE = 12f;
        private const float CORNER_RADIUS = 2.0f;

        private readonly Layout layout;
        public SKSize DesiredSize { get; private set; }

        public Thickness Padding { get; init; }

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

        public void Measure(SKSize constraint)
        {
            var contentConstraint = constraint;
            contentConstraint.Width -= (float)(Padding.Left + Padding.Right);
            contentConstraint.Height -= (float)(Padding.Top + Padding.Bottom);

            layout.Measure(contentConstraint);

            var contentSize = layout.DesiredSize;

            DesiredSize = new SKSize(
                constraint.Width,
                (float)(contentSize.Height + Padding.Top + Padding.Bottom)
            );
        }

        public void Paint(SKCanvas canvas, SKPoint point)
        {
            // using var paint = new SKPaint
            // {
            //     Color = SKColors.Aqua,
            //     Style = SKPaintStyle.Fill
            // };
            //
            // canvas.DrawRect(SKRect.Create(point, DesiredSize), paint);

            // paint.Color = backgroundColor;
            // paint.Style = SKPaintStyle.Fill;
            //
            // canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);
            //
            // paint.Color = borderColor;
            // paint.Style = SKPaintStyle.Stroke;
            //
            // canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);

            layout.Paint(canvas, point + new SKPoint((float)Padding.Left, (float)Padding.Top));
        }
    }
}
