using System.Windows.Media;

namespace Catalog.Wpf.ViewModel
{
    public class ImageViewModel : NotifyPropertyChangedBase
    {
        private string path;
        private ImageSource thumbnailSource;

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

        public ImageSource ThumbnailSource
        {
            get => thumbnailSource;
            set
            {
                if (Equals(value, thumbnailSource)) return;
                thumbnailSource = value;
                OnPropertyChanged();
            }
        }
    }
}