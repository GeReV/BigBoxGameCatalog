using System;
using Catalog.Forms.Controls;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Desktop
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var platform = new Eto.Wpf.Platform();

//            platform.Add<RichListBox.IHandler>(() => new RichListBoxHandler());

            new CatalogApplication(platform).Run(new MainForm());
        }
    }
}