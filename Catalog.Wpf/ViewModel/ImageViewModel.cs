using System.Windows.Media;
using Catalog.Model;

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

        public Image BuildImage()
        {
            return new Image
            {
                Path = Path
            };
        }
    }
}