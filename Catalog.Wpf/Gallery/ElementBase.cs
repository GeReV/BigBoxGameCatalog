using System.Windows;
using System.Windows.Data;
using SkiaSharp;

namespace Catalog.Wpf.Gallery
{
    public abstract class ElementBase : DependencyObject, IPaintable
    {
        private bool arrangeDirty = true;
        private bool measureDirty = true;
        public ElementBase? Parent { get; set; }

        #region IPaintable Members

        public SKSize DesiredSize { get; protected set; }

        public SKSize RenderSize { get; protected set; }

        public virtual void Measure(SKSize constraint)
        {
            DesiredSize = SKSize.Empty;

            measureDirty = false;
        }

        public virtual void Arrange(SKSize finalSize)
        {
            RenderSize = finalSize;

            arrangeDirty = false;
        }

        public virtual void InvalidateMeasure()
        {
            measureDirty = true;
        }

        public virtual void InvalidateArrange()
        {
            arrangeDirty = true;
        }

        public abstract void Paint(SKCanvas canvas, SKPoint point);

        #endregion

        /// <summary>
        ///     Attach a data Binding to the property
        /// </summary>
        /// <param name="dp">DependencyProperty that represents the property</param>
        /// <param name="binding">description of Binding to attach</param>
        public BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding)
        {
            return BindingOperations.SetBinding(this, dp, binding);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.GetMetadata(this) is FrameworkPropertyMetadata fmetadata)
            {
                var affectsParentMeasure = fmetadata.AffectsParentMeasure;
                var affectsParentArrange = fmetadata.AffectsParentArrange;
                var affectsMeasure = fmetadata.AffectsMeasure;
                var affectsArrange = fmetadata.AffectsArrange;
                if (affectsMeasure || affectsArrange || affectsParentArrange || affectsParentMeasure)
                {
                    var element = this;

                    while (element.Parent != null)
                    {
                        // if (affectsParentMeasure)
                        // {
                        //     parent.InvalidateMeasure();
                        // }
                        //
                        // if (affectsParentArrange)
                        // {
                        //     parent.InvalidateArrange();
                        // }

                        element = element.Parent;
                    }

                    if (affectsParentArrange)
                    {
                        element.InvalidateArrange();
                    }
                }

                if (fmetadata.AffectsMeasure)
                {
                    InvalidateMeasure();
                }

                if (fmetadata.AffectsArrange)
                {
                    InvalidateArrange();
                }

                if (fmetadata.AffectsRender)
                {
                    // InvalidateVisual();
                }
            }
        }
    }
}
