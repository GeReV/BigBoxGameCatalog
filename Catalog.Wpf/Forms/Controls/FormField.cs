using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Catalog.Wpf.Forms.Controls
{
    public class FormField : ContentControl
    {
        static FormField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormField), new FrameworkPropertyMetadata(typeof(FormField)));
            IsTabStopProperty.OverrideMetadata(typeof(FormField), new FrameworkPropertyMetadata(false));
        }

        #region ContentPadding

        /// <summary>
        /// ContentPadding Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register("ContentPadding", typeof(Thickness), typeof(FormField),
                new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the ContentPadding property.This dependency property
        /// indicates the padding for the control that is being labeled.
        /// </summary>
        [Category("Layout")]
        public static Thickness GetContentPadding(DependencyObject d)
        {
            return (Thickness) d.GetValue(ContentPaddingProperty);
        }

        /// <summary>
        /// Sets the ContentPadding property.This dependency property
        /// indicates the padding for the control that is being labeled.
        /// </summary>
        public static void SetContentPadding(DependencyObject d, Thickness value)
        {
            d.SetValue(ContentPaddingProperty, value);
        }

        #endregion

        #region LabelPadding

        /// <summary>
        /// LabelPadding Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelPaddingProperty =
            DependencyProperty.Register("LabelPadding", typeof(Thickness), typeof(FormField),
                new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the LabelPadding property.This dependency property
        /// indicates the padding for the label.
        /// </summary>
        [Category("Layout")]
        public static Thickness GetLabelPadding(DependencyObject d)
        {
            return (Thickness) d.GetValue(LabelPaddingProperty);
        }

        /// <summary>
        /// Sets the LabelPadding property.This dependency property
        /// indicates the padding for the label.
        /// </summary>
        public static void SetLabelPadding(DependencyObject d, Thickness value)
        {
            d.SetValue(LabelPaddingProperty, value);
        }

        #endregion

        #region LabelWidth

        /// <summary>
        /// LabelWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(double), typeof(FormField),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the LabelWidth property.This dependency property
        /// indicates the width for the label.
        /// </summary>
        [Category("Layout")]
        public static double GetLabelWidth(DependencyObject d)
        {
            return (double) d.GetValue(LabelWidthProperty);
        }

        /// <summary>
        /// Sets the LabelWidth property.This dependency property
        /// indicates the width for the label.
        /// </summary>
        public static void SetLabelWidth(DependencyObject d, double value)
        {
            d.SetValue(LabelWidthProperty, value);
        }

        #endregion

        #region LabelContent

        /// <summary>
        /// LabelContent Dependency Property
        /// </summary>
        [TypeConverter(typeof(StringConverter))]
        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(object), typeof(FormField),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets the LabelContent property.This dependency property
        /// indicates the label content.
        /// </summary>
        [Category("Layout")]
        [TypeConverter(typeof(StringConverter))]
        public static object GetLabelContent(DependencyObject d)
        {
            return d.GetValue(LabelContentProperty);
        }

        /// <summary>
        /// Sets the LabelContent property.This dependency property
        /// indicates the label content.
        /// </summary>
        [TypeConverter(typeof(StringConverter))]
        public static void SetLabelContent(DependencyObject d, object value)
        {
            d.SetValue(LabelContentProperty, value);
        }

        #endregion

        #region LabelContentTemplate

        /// <summary>
        /// LabelContentTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelContentTemplateProperty =
            DependencyProperty.Register("LabelContentTemplate", typeof(DataTemplate), typeof(FormField),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the LabelContentTemplate property.This dependency property
        /// indicates the label content template.
        /// </summary>
        public static DataTemplate GetLabelContentTemplate(DependencyObject d)
        {
            return (DataTemplate) d.GetValue(LabelContentTemplateProperty);
        }

        /// <summary>
        /// Sets the LabelContentTemplate property.This dependency property
        /// indicates the label content template.
        /// </summary>
        public static void SetLabelContentTemplate(DependencyObject d, DataTemplate value)
        {
            d.SetValue(LabelContentTemplateProperty, value);
        }

        #endregion

        #region LabelContentTemplateSelector

        /// <summary>
        /// LabelContentTemplateSelector Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelContentTemplateSelectorProperty =
            DependencyProperty.Register("LabelContentTemplateSelector", typeof(DataTemplateSelector),
                typeof(FormField), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the LabelContentTemplateSelector property.This dependency property
        /// indicates the content template selector for labels.
        /// </summary>
        public static DataTemplateSelector GetLabelContentTemplateSelector(DependencyObject d)
        {
            return (DataTemplateSelector) d.GetValue(LabelContentTemplateSelectorProperty);
        }

        /// <summary>
        /// Sets the LabelContentTemplateSelector property.This dependency property
        /// indicates the content template selector for labels.
        /// </summary>
        public static void SetLabelContentTemplateSelector(DependencyObject d, DataTemplateSelector value)
        {
            d.SetValue(LabelContentTemplateSelectorProperty, value);
        }

        #endregion
    }
}