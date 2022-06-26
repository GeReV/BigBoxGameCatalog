using System.Windows;

namespace Catalog.Wpf.Gallery
{
    public abstract class BoxElement : ElementBase
    {
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            nameof(Padding),
            typeof(Thickness),
            typeof(BoxElement),
            new FrameworkPropertyMetadata(
                default(Thickness),
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange
            )
        );

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }
    }
}
