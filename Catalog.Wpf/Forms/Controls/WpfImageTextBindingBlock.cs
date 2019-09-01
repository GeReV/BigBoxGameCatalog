using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Forms.Controls;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;

namespace Catalog.Wpf.Forms.Controls
{
    public class WpfImageTextSubtitleBindingBlock : sw.FrameworkElementFactory
    {
        public WpfImageTextSubtitleBindingBlock(Func<IIndirectBinding<string>> textBinding, Func<IIndirectBinding<string>> subtitleBinding, Func<IIndirectBinding<Image>> imageBinding, bool editable, swd.RelativeSource relativeSource = null)
            : base(typeof(swc.StackPanel))
        {
            SetValue(swc.StackPanel.OrientationProperty, swc.Orientation.Horizontal);

            AppendChild(new WpfImageBindingBlock(imageBinding));

            var titles = new sw.FrameworkElementFactory(typeof(swc.StackPanel));
            titles.SetValue(swc.StackPanel.OrientationProperty, swc.Orientation.Vertical);

            AppendChild(titles);

            if (editable)
            {
                titles.AppendChild(new WpfEditableTextBindingBlock(textBinding, relativeSource));
                titles.AppendChild(new WpfEditableTextBindingBlock(subtitleBinding, relativeSource));
            }
            else
            {
                titles.AppendChild(new WpfTextBindingBlock(textBinding));

                var subtitle = new WpfTextBindingBlock(subtitleBinding);

                subtitle.SetValue(swc.TextBlock.FontSizeProperty, 10D);

                titles.AppendChild(subtitle);
            }
        }
    }
}