using System.Windows;
using SkiaSharp;
using Topten.RichTextKit;
using Style = Topten.RichTextKit.Style;
using TextAlignment = Topten.RichTextKit.TextAlignment;

namespace Catalog.Wpf.Gallery
{
    public class Tag : IBox
    {
        private string text;
        public SKColor Color { get; set; }

        private readonly TextBlock textBlock;
        private readonly Style style = new()
        {
            FontFamily = "system",
            FontSize = 10.0f
        };

        public string Text
        {
            get => text;
            set
            {
                text = value;
                UpdateTextBlock();
            }
        }
        
        public SKSize DesiredSize { get; private set; }
        
        public Thickness Padding { get; init; }

        public Tag(string text, SKColor color)
        {
            this.text = text;
            
            Color = color;
            Padding = new Thickness(2f, 0f, 2f, 0f);

            textBlock = new TextBlock
            {
                MaxLines = 1
            };

            UpdateTextBlock();
        }

        private void UpdateTextBlock()
        {
            textBlock.Clear();
            // textBlock.AddText(Text, style.Modify(textColor: Color.GetLuminance() > 0.5f ? SKColors.Black : SKColors.White));
        }

        public void Measure(SKSize constraint)
        {
            var horizontalPadding = Padding.Left + Padding.Right;
            
            textBlock.MaxWidth = (float)(constraint.Width - horizontalPadding);
            textBlock.MaxHeight = constraint.Height;

            DesiredSize = new SKSize(
                (float)(textBlock.MeasuredWidth + horizontalPadding),
                (float)(textBlock.MeasuredHeight + Padding.Top + Padding.Bottom)
            );
        }

        public void Paint(SKCanvas canvas, SKPoint point)
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
                Style = SKPaintStyle.Fill,
            };
            
            var offset = new SKPoint((float)Padding.Left, (float)Padding.Top);
            
            canvas.DrawRoundRect(rect, 3.0f, 3.0f, paint);

            textBlock.Paint(canvas, point + offset);
        }
    }
}
