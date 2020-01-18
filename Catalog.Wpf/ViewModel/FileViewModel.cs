using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class FileViewModel : NotifyPropertyChangedBase, ICloneable<FileViewModel>
    {
        private string path;
        private byte[] sha256Checksum;
        private int progress;

        public FileViewModel(string path, byte[] sha256Checksum, Progress<int>? progress = null)
        {
            this.path = path;
            this.sha256Checksum = sha256Checksum;

            if (progress == null)
            {
                return;
            }

            progress.ProgressChanged += (_, percentage) => { Progress = percentage; };
        }

        public string Path
        {
            get => path;
            set
            {
                if (value == path) return;
                path = value;
                OnPropertyChanged();
            }
        }

        public byte[] Sha256Checksum
        {
            get => sha256Checksum;
            set
            {
                if (Equals(value, sha256Checksum)) return;
                sha256Checksum = value;
                OnPropertyChanged();
            }
        }

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

        public File BuildFile()
        {
            return new File
            {
                Path = Path,
                Sha256Checksum = Sha256Checksum
            };
        }

        public static FileViewModel FromFile(File file) =>
            new FileViewModel(file.Path, file.Sha256Checksum);

        public FileViewModel Clone() => new FileViewModel(Path, Sha256Checksum);
    }
}
