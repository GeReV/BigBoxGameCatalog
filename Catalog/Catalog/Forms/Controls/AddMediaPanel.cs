using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Catalog.Model;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Forms.Controls
{
    public class AddMediaPanel : Panel
    {
        private Dictionary<ItemType, NumericStepper> steppers = new Dictionary<ItemType, NumericStepper>();

        public AddMediaPanel()
        {
            var mediaTypes = typeof(ItemTypes).GetMembers()
                .Where(memberInfo => memberInfo.GetCustomAttribute<CategoryAttribute>()?.Category == "Media")
                .Select(memberInfo => ((FieldInfo) memberInfo).GetValue(null))
                .Cast<ItemType>();

            var rows = new List<TableRow>();

            foreach (var type in mediaTypes)
            {
                var stepper = new NumericStepper
                {
                    MinValue = 0,
                    Width = 100,
                };

                stepper.ValueChanged += StepperOnValueChanged;

                steppers.Add(type, stepper);

                rows.Add(new TableRow(
                    type.Description,
                    stepper,
                    null
                ));
            }

            Content = new TableLayout(rows)
            {
                Spacing = new Size(5,5),
            };
        }

        private void StepperOnValueChanged(object sender, EventArgs e)
        {

        }

        public Dictionary<ItemType, int> MediaValues
        {
            get
            {
                var dict = new Dictionary<ItemType, int>();

                foreach (var pair in steppers)
                {
                    dict.Add(pair.Key, (int)pair.Value.Value);
                }

                return dict;
            }
            set
            {
                foreach (var pair in value)
                {
                    steppers[pair.Key].Value = pair.Value;
                }
            }
        }

        public void SetStepperValue(ItemType mediaType, int value)
        {
            if (steppers.ContainsKey(mediaType))
            {
                steppers[mediaType].Value = value;
            }
        }
    }
}