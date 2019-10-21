using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Catalog.Wpf.ViewModel;
using Microsoft.Win32;

namespace Catalog.Wpf.Commands
{
    public class SelectCoverImageCommand : CommandBase
    {
        private readonly EditGameViewModel editGameViewModel;

        public SelectCoverImageCommand(EditGameViewModel editGameViewModel)
        {
            this.editGameViewModel = editGameViewModel;
        }

        public override void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter =
                    "Image files (*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.tif;*.tiff;*.tga;)|*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.tif;*.tiff;*.tga;",
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            editGameViewModel.GameCoverImage = new ScreenshotViewModel
            {
                Url = openFileDialog.FileName,
                ThumbnailUrl = openFileDialog.FileName
            };
        }
    }
}