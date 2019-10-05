using System.Windows.Input;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Forms.Controls
{
    public class AddRemoveButtons : Panel
    {
        private StackLayout stackLayout = new StackLayout
        {
            Spacing = 5,
        };

        private Button addButton = new Button
        {
//            Image = new Bitmap(Icons.plus),
            MinimumSize = new Size(16, 16)
        };

        private Button removeButton = new Button
        {
//            Image = new Bitmap(Icons.minus),
            MinimumSize = new Size(16, 16)
        };

        public AddRemoveButtons()
        {
            stackLayout.Items.Add(addButton);
            stackLayout.Items.Add(removeButton);

            Content = stackLayout;
        }

        public Orientation Orientation
        {
            get => stackLayout.Orientation;
            set => stackLayout.Orientation = value;
        }

        public ICommand AddCommand
        {
            get => addButton.Command;
            set => addButton.Command = value;
        }

        public ICommand RemoveCommand
        {
            get => removeButton.Command;
            set => removeButton.Command = value;
        }
    }
}