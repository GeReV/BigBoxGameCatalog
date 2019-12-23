using System;
using System.Windows.Media.Imaging;
using Catalog.Wpf.ViewModel;
using Microsoft.Win32;

namespace Catalog.Wpf.Commands
{
    public class GameItemAddScanCommand : CommandBase
    {
        private readonly ItemViewModel itemViewModel;

        public GameItemAddScanCommand(ItemViewModel itemViewModel)
        {
            this.itemViewModel = itemViewModel;
        }

        public override void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                DereferenceLinks = true,
                Filter =
                    "Image files (*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.tif;*.tiff;*.tga;*.pdf)|*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.tif;*.tiff;*.tga;*.pdf"
            };

            // TODO: Handle PDF thumbnails.

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            foreach (var fileName in openFileDialog.FileNames)
            {
                var imageSource = new ImageViewModel(fileName, new BitmapImage(new Uri(fileName)));

                itemViewModel.Scans.Add(imageSource);
            }
        }
    }
}
