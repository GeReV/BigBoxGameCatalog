using System;
using System.Diagnostics;
using System.IO;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class OpenFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var explorerPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");

            switch (parameter)
            {
                case ILocalResource resource:
                    Process.Start(explorerPath, resource.Path);
                    break;
                case ScreenshotViewModel screenshot:
                    Process.Start(explorerPath, screenshot.Url);
                    break;
            }
        }
    }
}
