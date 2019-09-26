using System;
using Eto.Forms;

namespace Catalog.Forms
{
    public static class DynamicLayoutHelpers
    {
        public static void AddLabeledRow(this DynamicLayout layout, string label, Control control, bool? yscale = null)
        {
            layout.AddLabeledRow(label, l => l.Add(control, true, yscale));
        }

        public static void AddLabeledRow(this DynamicLayout layout, string label, Action<DynamicLayout> func)
        {
            layout.BeginHorizontal();
            layout.Add(new Label {Text = label, Width = 200});
            func(layout);
            layout.EndHorizontal();
        }
    }
}