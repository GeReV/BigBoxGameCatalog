using System;
using SkiaSharp;
using Topten.RichTextKit;

namespace Catalog.Wpf.Gallery
{
    public class TextLine : IPaintable
    {
        private IStyle highlightTextStyle;
        private TextBlock textBlock;
        private SKColor highlightTextColor = SKColors.Goldenrod;

        public SKSize DesiredSize { get; private set; }
        public string Text { get; }
        public string? HighlightedText { get; set; }
        public Style TextStyle { get; set; }
        public SKColor HighlightTextColor
        {
            get => highlightTextColor;
            set
            {
                highlightTextColor = value;
                
                highlightTextStyle = TextStyle.Modify(backgroundColor: value);
            }
        }

        public TextLine(string text, Style textStyle)
        {
            textBlock = new TextBlock
            {
                MaxLines = 1,
                Alignment = TextAlignment.Center,
            };

            Text = text;
            TextStyle = textStyle;
            highlightTextStyle = TextStyle.Modify(backgroundColor: HighlightTextColor);
        }

        public void Measure(SKSize constraint)
        {
            textBlock.MaxWidth = constraint.Width;
            textBlock.MaxHeight = constraint.Height;
            
            textBlock.Clear();
            textBlock.AddText(Text, TextStyle);

            DesiredSize = new SKSize(textBlock.MeasuredWidth, textBlock.MeasuredHeight);
        }

        public void Paint(SKCanvas canvas, SKPoint point)
        {
            var term = HighlightedText;

            textBlock.Clear();
            textBlock.AddText(Text, TextStyle);

            // We add the pre-truncated text ourselves, since trying to style the full text yields unexpected results
            // together with the automated truncation.
            var truncatedText = Text[..textBlock.MeasuredLength].Trim();

            textBlock.Clear();
            textBlock.AddText(truncatedText, TextStyle);

            if (truncatedText.Length < Text.Length)
            {
                textBlock.AddText("…", TextStyle);
            }

            if (!string.IsNullOrWhiteSpace(term))
            {
                var index = 0;

                while (index < truncatedText.Length)
                {
                    var matchIndex = Text.IndexOf(term, index, StringComparison.InvariantCultureIgnoreCase);

                    if (matchIndex < 0)
                    {
                        break;
                    }

                    if (matchIndex > truncatedText.Length)
                    {
                        break;
                    }

                    var len = Math.Min(term.Length, truncatedText.Length - matchIndex);

                    textBlock.ApplyStyle(
                        matchIndex,
                        len,
                        highlightTextStyle
                    );

                    index = matchIndex + len;
                }
            }

            textBlock.Paint(canvas, point);
        }
    }
}
