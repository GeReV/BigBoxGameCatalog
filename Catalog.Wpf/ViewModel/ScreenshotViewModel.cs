using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = Catalog.Model.Image;

namespace Catalog.Wpf.ViewModel
{
    public sealed class ScreenshotViewModel : NotifyPropertyChangedBase
    {
        private Uri thumbnailUrl;
        private Uri url;

        public ImageSource? ThumbnailSource
        {
            get
            {
                try
                {
                    return new BitmapImage(ThumbnailUrl);
                }
                catch (IOException)
                {
                    return null;
                }
            }
        }

        public ScreenshotViewModel(Uri thumbnailUrl, Uri url)
        {
            this.thumbnailUrl = thumbnailUrl;
            this.url = url;
        }

        public Uri ThumbnailUrl
        {
            get => thumbnailUrl;
            set
            {
                if (value == thumbnailUrl) return;
                thumbnailUrl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThumbnailSource));
            }
        }

        public Uri Url
        {
            get => url;
            set
            {
                if (value == url) return;
                url = value;
                OnPropertyChanged();
            }
        }

        public static ScreenshotViewModel FromPath(Uri path) => new(path, path);
        public static ScreenshotViewModel FromPath(string path) => FromPath(new Uri(path));

        public static ScreenshotViewModel FromImage(Image image) => FromPath(new Uri(image.Path));
    }
}
