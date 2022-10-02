using System;
using System.Drawing;
using System.Windows;
using Catalog.Wpf.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Topten.RichTextKit;
using Style = Topten.RichTextKit.Style;

namespace Catalog.Wpf.Gallery
{
    public sealed class Tag : BoxElement
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(Tag),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange,
                UpdateTextCallback
            )
        );

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color),
            typeof(object),
            typeof(Tag),
            new FrameworkPropertyMetadata(
                SKColor.Empty,
                FrameworkPropertyMetadataOptions.AffectsArrange,
                UpdateTextCallback,
                CoerceColorValueCallback
            )
        );

        private readonly Style style = new()
        {
            FontFamily = "system",
            FontSize = 10.0f
        };

        private readonly TextBlock textBlock;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public SKColor Color
        {
            get => (SKColor)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        static Tag()
        {
            PaddingProperty.OverrideMetadata(typeof(Tag), new FrameworkPropertyMetadata(new Thickness(2f, 0, 2f, 1f)));
        }

        public Tag()
        {
            textBlock = new TextBlock
            {
                MaxLines = 1
            };
        }

        private static void UpdateTextCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tag = (Tag)d;

            tag.UpdateTextBlock();
        }

        private static object CoerceColorValueCallback(DependencyObject d, object value)
        {
            return value switch
            {
                Color c => c.ToSKColor(),
                SKColor c => c,
                _ => throw new ArgumentException("Expected color")
            };
        }

        public override void Measure(SKSize constraint)
        {
            var horizontalPadding = (float)(Padding.Left + Padding.Right);

            textBlock.MaxWidth = constraint.Width - horizontalPadding;
            textBlock.MaxHeight = constraint.Height;

            DesiredSize = new SKSize(
                textBlock.MeasuredWidth + horizontalPadding,
                (float)(textBlock.MeasuredHeight + Padding.Top + Padding.Bottom)
            );
        }

        public override void Paint(SKCanvas canvas, SKPoint point)
        {
            var rect = new SKRect
            {
                Location = point,
                Size = new SKSize(
                    (float)(textBlock.MeasuredWidth + Padding.Left + Padding.Right),
                    (float)(textBlock.MeasuredHeight + Padding.Top + Padding.Bottom)
                )
            };

            using var paint = new SKPaint
            {
                Color = Color,
                Style = SKPaintStyle.Fill
            };

            var offset = new SKPoint((float)Padding.Left, (float)Padding.Top);

            canvas.DrawRoundRect(rect, 3.0f, 3.0f, paint);

            textBlock.Paint(canvas, point + offset);
        }

        private void UpdateTextBlock()
        {
            textBlock.Clear();
            textBlock.AddText(
                Text,
                style.Modify(textColor: Color.GetLuminance() > 0.5f ? SKColors.Black : SKColors.White)
            );
        }
    }
}
