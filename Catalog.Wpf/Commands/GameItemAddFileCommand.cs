using System;
using System.Threading.Tasks;
using Catalog.Wpf.ViewModel;
using Microsoft.Win32;

namespace Catalog.Wpf.Commands
{
    public class GameItemAddFileCommand : CommandBase
    {
        private readonly ItemViewModel itemViewModel;

        public GameItemAddFileCommand(ItemViewModel itemViewModel)
        {
            this.itemViewModel = itemViewModel;
        }

        public override void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                DereferenceLinks = true,
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            foreach (var fileName in openFileDialog.FileNames)
            {
                var inputStream = System.IO.File.OpenRead(fileName);

                var progress = new Progress<int>();

                var file = new FileViewModel(fileName, new byte[] {}, progress);

                itemViewModel.Files.Add(file);

                Checksum
                    .GenerateSha256Async(inputStream, progress)
                    .ContinueWith(
                        hashTask =>
                        {
                            inputStream.Dispose();

                            file.Sha256Checksum = hashTask.Result;
                        },
                        TaskScheduler.FromCurrentSynchronizationContext()
                    );
            }
        }
    }
}
