using System;
using Eto.Forms;

namespace Catalog.Forms
{
    public static class DynamicLayoutHelpers
    {
        public static void AddLabeledRow(this DynamicLayout layout, string label, Control control, bool? yscale = null, int labelWidth = 200)
        {
            layout.AddLabeledRow(label, l => l.Add(control, true, yscale), labelWidth);
        }

        public static void AddLabeledRow(this DynamicLayout layout, string label, Action<DynamicLayout> func, int labelWidth = 200)
        {
            layout.BeginHorizontal();
            layout.Add(new Label {Text = label, Width = labelWidth });
            func(layout);
            layout.EndHorizontal();
        }
    }
}