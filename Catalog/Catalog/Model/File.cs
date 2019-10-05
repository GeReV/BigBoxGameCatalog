using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    public class File : NotifyPropertyChangedBase
    {
        private string path;
        private byte[] sha256Checksum;

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
    }
}
