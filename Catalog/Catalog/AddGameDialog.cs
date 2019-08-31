using System;
using Eto.Forms;
using Eto.Drawing;
using Catalog.Model;

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
