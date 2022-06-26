using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Catalog.Wpf.Forms;
using Catalog.Wpf.ViewModel;
using SkiaSharp;
using Style = Topten.RichTextKit.Style;

namespace Catalog.Wpf.Gallery
{
    public sealed class GalleryItem : Layout
    {
        private const float LINE_MARGIN = 4f;
        private const float FONT_SIZE = 12f;
        private const float CORNER_RADIUS = 2.0f;

        private readonly UIElement parent;
        public bool IsHighlighted { get; set; }
        public bool IsSelected { get; set; }
        public SKColor BorderColor { get; set; } = new(0xff70c0e7);
        public SKColor HighlightBackgroundColor { get; set; } = new(0xffe5f3fb);
        public SKColor SelectedBackgroundColor { get; set; } = new(0xffcbe8f6);

        public GalleryItem(UIElement parent, GameViewModel game, SkiaTextureAtlas textureAtlas)
        {
            this.parent = parent;

            InitializeComponents(game, textureAtlas);
        }

        private void InitializeComponents(GameViewModel game, SkiaTextureAtlas textureAtlas)
        {
            var textStyle = new Style
            {
                FontFamily = "system",
                FontSize = FONT_SIZE
            };

            var textLine = new TextLine
            {
                TextStyle = textStyle
            };

            textLine.SetBinding(
                TextLine.TextProperty,
                new Binding(nameof(GameViewModel.Title))
                {
                    Source = game
                }
            );

            textLine.SetBinding(
                TextLine.HighlightedTextProperty,
                new Binding(nameof(GameGalleryView.HighlightedText))
                {
                    Source = parent
                }
            );

            var tags = game.Tags.Select(
                t =>
                {
                    var tag = new Tag();

                    tag.SetBinding(
                        Tag.TextProperty,
                        new Binding(nameof(Model.Tag.Name))
                        {
                            Source = t
                        }
                    );

                    tag.SetBinding(
                        Tag.ColorProperty,
                        new Binding(nameof(Model.Tag.Color))
                        {
                            Source = t
                        }
                    );

                    return tag;
                }
            );

            var layout = new StackingLayout(
                Orientation.Vertical,
                new ElementBase[]
                {
                    new Image(textureAtlas, game.CoverPath),
                    textLine,
                    new WrappingLayout(tags)
                    {
                        HorizontalSpacing = 4.0f,
                        VerticalSpacing = 2.0f
                    }
                }
            )
            {
                Spacing = LINE_MARGIN
            };

            AddItem(layout);
        }

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();

            parent.InvalidateMeasure();
        }

        public override void InvalidateArrange()
        {
            base.InvalidateArrange();

            parent.InvalidateArrange();
        }

        public override void Measure(SKSize constraint)
        {
            var contentConstraint = constraint;
            contentConstraint.Width -= (float)(Padding.Left + Padding.Right);
            contentConstraint.Height -= (float)(Padding.Top + Padding.Bottom);

            base.Measure(contentConstraint);

            var height = 0f;

            foreach (var item in Items)
            {
                item.Measure(contentConstraint);

                if (item.DesiredSize.Height > height)
                {
                    height = item.DesiredSize.Height;
                }
            }

            DesiredSize = new SKSize(
                constraint.Width,
                (float)Math.Ceiling(height + Padding.Top + Padding.Bottom)
            );
        }

        public override void Arrange(SKSize finalSize)
        {
            base.Arrange(finalSize);

            var contentSize = new SKSize(
                (float)(finalSize.Width - Padding.Left - Padding.Right),
                (float)(finalSize.Height - Padding.Top - Padding.Bottom)
            );

            foreach (var item in Items)
            {
                item.Arrange(contentSize);
            }
        }

        public override void Paint(SKCanvas canvas, SKPoint point)
        {
            DrawBackground(canvas, point);

            var position = point + new SKPoint((float)Padding.Left, (float)Padding.Top);

            foreach (var item in Items)
            {
                item.Paint(canvas, position);
            }
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

            var rect = SKRect.Create(position, RenderSize);

            paint.Color = backgroundColor;
            paint.Style = SKPaintStyle.Fill;

            canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);

            paint.Color = BorderColor;
            paint.Style = SKPaintStyle.Stroke;

            canvas.DrawRoundRect(rect, CORNER_RADIUS, CORNER_RADIUS, paint);
        }
    }
}
