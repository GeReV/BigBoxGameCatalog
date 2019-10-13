using System.Windows;
using System.Windows.Documents;
using Catalog.Wpf.Annotations;

namespace Catalog.Wpf.Forms.Controls
{
    public class Highlight : Span
    {
        static Highlight()
        {
            FrameworkContentElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Highlight),
                (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof(Highlight)));
        }

        public Highlight()
        {
        }

        public Highlight(Inline childInline) : base(childInline)
        {
        }

        public Highlight(Inline childInline, TextPointer insertionPosition) : base(childInline, insertionPosition)
        {
        }

        public Highlight([NotNull] TextPointer start, [NotNull] TextPointer end) : base(start, end)
        {
        }
    }
}