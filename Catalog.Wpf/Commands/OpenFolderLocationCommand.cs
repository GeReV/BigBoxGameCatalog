using System.Diagnostics;
using Catalog.Model;

namespace Catalog.Wpf.Commands
{
    public class OpenFolderLocationCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if (!(parameter is LocalResource resource))
            {
                return;
            }

            Process.Start("explorer.exe", $"/select,\"{resource.Path}\"");
        }
    }
}