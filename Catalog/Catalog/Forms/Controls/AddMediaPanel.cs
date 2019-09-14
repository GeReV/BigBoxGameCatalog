using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Model;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Forms.Controls
{
    public class AddMediaPanel : Panel
    {
        private Dictionary<MediaType, NumericStepper> steppers = new Dictionary<MediaType, NumericStepper>();
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var mediaTypes = Enum.GetValues(typeof(MediaType)).Cast<MediaType>().ToList();

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
                    type.GetDescription(),
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

        public Dictionary<MediaType, int> MediaValues
        {
            get
            {
                var dict = new Dictionary<MediaType, int>();

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

        public void SetStepperValue(MediaType mediaType, int value)
        {
            if (steppers.ContainsKey(mediaType))
            {
                steppers[mediaType].Value = value;
            }
        }
    }
}