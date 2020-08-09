using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = Catalog.Model.Image;

namespace Catalog.Wpf.ViewModel
{
    public sealed class ScreenshotViewModel : NotifyPropertyChangedBase
    {
        private string thumbnailUrl;
        private string url;

        public ImageSource? ThumbnailSource
        {
            get {
                try
                {
                    return new BitmapImage(new Uri(ThumbnailUrl));
                }
                catch (IOException)
                {
                    return null;
                }
            }
        }

        public ScreenshotViewModel(string thumbnailUrl, string url)
        {
            this.thumbnailUrl = thumbnailUrl;
            this.url = url;
        }

        public string ThumbnailUrl
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

        public string Url
        {
            get => url;
            set
            {
                if (value == url) return;
                url = value;
                OnPropertyChanged();
            }
        }

        public static ScreenshotViewModel FromPath(string path) =>
            new ScreenshotViewModel(path, path);

        public static ScreenshotViewModel FromImage(Image image) =>
            new ScreenshotViewModel(image.Path, image.Path);
    }
}
