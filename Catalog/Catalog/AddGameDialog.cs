using System;
using Catalog.Model;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog
{
    public partial class AddGameDialog : Dialog<GameCopy>
    {
        public AddGameDialog()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            DefaultControl.Focus();
        }
    }
}
