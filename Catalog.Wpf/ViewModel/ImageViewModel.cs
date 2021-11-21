﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class ImageViewModel : NotifyPropertyChangedBase, ICloneable<ImageViewModel>
    {
        private int imageId;
        private string path;
        private ImageSource thumbnailSource;

        public ImageViewModel(string path, ImageSource thumbnailSource)
        {
            this.path = path;
            this.thumbnailSource = thumbnailSource;
        }

        public int ImageId
        {
            get => imageId;
            set
            {
                if (value == imageId) return;
                imageId = value;
                OnPropertyChanged();
            }
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
            return new Image(Path)
            {
                ImageId = ImageId,
            };
        }

        public static ImageViewModel FromImage(Image image) =>
            new ImageViewModel(image.Path, new BitmapImage(new Uri(image.Path)))
            {
                ImageId = image.ImageId
            };

        public ImageViewModel Clone() => new ImageViewModel(Path, thumbnailSource.Clone());
    }
}
