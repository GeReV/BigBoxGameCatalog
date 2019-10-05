using System;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class FileViewModel : NotifyPropertyChangedBase
    {
        private readonly File file;
        private int progress;

        public FileViewModel(File file, Progress<int> progress)
        {
            this.file = file;
            this.file.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(File));

            progress.ProgressChanged += (sender, args) =>
            {
                Progress = args;
            };
        }

        public File File => file;

        public int Progress
        {
            get => progress;
            private set
            {
                if (value == progress) return;
                progress = value;
                OnPropertyChanged();
            }
        }
    }
}