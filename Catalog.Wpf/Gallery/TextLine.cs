using System;
using System.Windows;
using SkiaSharp;
using Topten.RichTextKit;
using Style = Topten.RichTextKit.Style;
using TextAlignment = Topten.RichTextKit.TextAlignment;

namespace Catalog.Wpf.Gallery
{
    public sealed class TextLine : ElementBase
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TextLine),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange
            )
        );

        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register(
            nameof(TextStyle),
            typeof(Style),
            typeof(TextLine),
            new PropertyMetadata(default(Style), UpdateHighlightedTextStyle)
        );

        public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.Register(
            nameof(HighlightedText),
            typeof(string),
            typeof(TextLine),
            new FrameworkPropertyMetadata(default(string?), FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static readonly DependencyProperty HighlightedTextColorProperty = DependencyProperty.Register(
            nameof(HighlightedTextColor),
            typeof(SKColor),
            typeof(TextLine),
            new FrameworkPropertyMetadata(
                SKColors.PaleGoldenrod,
                FrameworkPropertyMetadataOptions.AffectsRender,
                UpdateHighlightedTextStyle
            )
        );

        private readonly TextBlock textBlock;

        private IStyle highlightTextStyle = new Style();

        public Style TextStyle
        {
            get => (Style)GetValue(TextStyleProperty);
            set => SetValue(TextStyleProperty, value);
        }

        public string? HighlightedText
        {
            get => (string?)GetValue(HighlightedTextProperty);
            set => SetValue(HighlightedTextProperty, value);
        }

        public SKColor HighlightedTextColor
        {
            get => (SKColor)GetValue(HighlightedTextColorProperty);
            set => SetValue(HighlightedTextColorProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextLine()
        {
            textBlock = new TextBlock
            {
                MaxLines = 1,
                Alignment = TextAlignment.Center
            };
        }

        private static void UpdateHighlightedTextStyle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textLine = (TextLine)d;

            textLine.highlightTextStyle = textLine.TextStyle.Modify(backgroundColor: textLine.HighlightedTextColor);
        }

        public override void Measure(SKSize constraint)
        {
            textBlock.MaxWidth = constraint.Width;
            textBlock.MaxHeight = constraint.Height;

            textBlock.Clear();
            textBlock.AddText(Text, TextStyle);

            DesiredSize = new SKSize(textBlock.MeasuredWidth, textBlock.MeasuredHeight);
        }

        public override void Paint(SKCanvas canvas, SKPoint point)
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
