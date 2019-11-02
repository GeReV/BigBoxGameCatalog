using System.Diagnostics;
using Catalog.Model;
using Catalog.Wpf.ViewModel;

namespace Catalog.Wpf.Commands
{
    public class OpenFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case LocalResource resource:
                    Process.Start(resource.Path);
                    break;
                case ScreenshotViewModel screenshot:
                    Process.Start(screenshot.Url);
                    break;
            }
        }
    }
}