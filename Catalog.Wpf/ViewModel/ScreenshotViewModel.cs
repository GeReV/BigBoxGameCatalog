using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = Catalog.Model.Image;

namespace Catalog.Wpf.ViewModel
{
    public sealed class ScreenshotViewModel : NotifyPropertyChangedBase
    {
        private string thumbnailUrl;
        private string url;

        public ImageSource ThumbnailSource => new BitmapImage(new Uri(ThumbnailUrl));

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
            new ScreenshotViewModel
            {
                Url = path,
                ThumbnailUrl = path
            };

        public static ScreenshotViewModel FromImage(Image image) =>
            new ScreenshotViewModel
            {
                Url = image.Path,
                ThumbnailUrl = image.Path
            };
    }
}
