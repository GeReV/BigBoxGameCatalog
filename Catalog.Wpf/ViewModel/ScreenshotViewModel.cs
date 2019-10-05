using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Catalog.Model;
using Catalog.Wpf.Annotations;
using Eto.Drawing;

namespace Catalog.Wpf.ViewModel
{
    public sealed class ScreenshotViewModel : NotifyPropertyChangedBase
    {
        private ImageSource thumbnailSource;
        private string thumbnailUrl;
        private string url;

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

        public string ThumbnailUrl
        {
            get => thumbnailUrl;
            set
            {
                if (value == thumbnailUrl) return;
                thumbnailUrl = value;
                OnPropertyChanged();
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
    }
}