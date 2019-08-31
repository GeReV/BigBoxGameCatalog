using System;
using Eto.Forms;
using Eto.Drawing;
using Catalog.Forms.Controls;
using Catalog.Wpf.Forms.Controls;

namespace Catalog.Desktop
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var platform = new Eto.Wpf.Platform();

            platform.Add<RichListBox.IHandler>(() => new RichListBoxHandler());

            new CatalogApplication(platform).Run(new MainForm());
        }
    }
}