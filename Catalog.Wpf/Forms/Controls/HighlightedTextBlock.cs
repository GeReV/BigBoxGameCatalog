using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class HighlightedTextBlock : TextBlock
    {
        public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.Register(
            "HighlightedText", typeof(string), typeof(HighlightedTextBlock),
            (PropertyMetadata) new UIPropertyMetadata(new PropertyChangedCallback(OnHighlightChanged)));

        public string HighlightedText
        {
            get => (string) GetValue(HighlightedTextProperty);
            set => SetValue(HighlightedTextProperty, value);
        }

        static HighlightedTextBlock()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightedTextBlock),
                (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof(HighlightedTextBlock)));
        }

        private static void OnHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = (TextBlock) d;

            UpdateText(textBlock, (string) textBlock.GetValue(TextProperty), (string) e.NewValue);
        }

        private static void UpdateText(TextBlock textBlock, string text, string highlight)
        {
            if (string.IsNullOrWhiteSpace(highlight))
            {
                textBlock.Text = text;

                return;
            }

            var r = new Regex(highlight, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            textBlock.Inlines.Clear();

            var index = 0;

            foreach (Match match in r.Matches(text))
            {
                textBlock.Inlines.Add(new Run(text.Substring(index, match.Index - index)));
                textBlock.Inlines.Add(new Highlight(new Run(match.Value)));

                index = match.Index + match.Length;
            }

            if (index < text.Length)
            {
                textBlock.Inlines.Add(new Run(text.Substring(index)));
            }
        }
    }
}