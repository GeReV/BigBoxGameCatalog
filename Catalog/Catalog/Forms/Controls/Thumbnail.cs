using System;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Forms
{
    public class Thumbnail : Panel
    {
        private bool isChecked = false;
        
        private ImageView imageView = new ImageView
        {
            Height = 120,
        };

        public Thumbnail()
        {
            MouseUp += (sender, args) =>
            {
                if (args.Buttons == MouseButtons.Primary)
                {
                    Checked = !Checked;
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            Padding = new Padding(5);

            Content = imageView;
        }

        private static readonly object CheckedChangedKey = new object();

        public event EventHandler<EventArgs> CheckedChanged
        {
            add => Properties.AddEvent(CheckedChangedKey, value);
            remove => Properties.AddEvent(CheckedChangedKey, value);
        }

        protected void TriggerCheckedChanged()
        {
            Properties.TriggerEvent(CheckedChangedKey, this, EventArgs.Empty);
        }

        public bool Checked
        {
            get => isChecked;
            set
            {
                if (value != isChecked)
                {
                    TriggerCheckedChanged();
                }

                BackgroundColor = value ? Colors.LightBlue : Colors.White;
                
                isChecked = value;
            }
        }

        public Image Image
        {
            get => imageView.Image;
            set => imageView.Image = value;
        }
    }
}