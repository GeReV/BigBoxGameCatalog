using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class GameViewModel : NotifyPropertyChangedBase
    {
        private GameCopy gameCopy;

        public GameViewModel(GameCopy gameCopy)
        {
            GameCopy = gameCopy;
        }

        public GameCopy GameCopy
        {
            get => gameCopy;
            set
            {
                if (Equals(value, gameCopy)) return;
                gameCopy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Publisher));
                OnPropertyChanged(nameof(Developers));
                OnPropertyChanged(nameof(Notes));
                OnPropertyChanged(nameof(Cover));
                OnPropertyChanged(nameof(GameStats));
            }
        }

        public string Title => GameCopy.Title;

        public Publisher Publisher => GameCopy.Publisher;

        public IList<Developer> Developers => GameCopy.Developers;

        public string Notes => GameCopy.Notes;

        public ImageSource Cover => GameCopy.CoverImage?.Path == null
            ? null
            : new BitmapImage(new Uri(GameCopy.CoverImage.Path));

        public IEnumerable<ScreenshotViewModel> Screenshots =>
            GameCopy.Screenshots.Select(ScreenshotViewModel.FromImage).ToList();

        public IEnumerable<GameItemGroupViewModel> GameStats => GameItemGrouping.GroupItems(GameCopy.Items);
    }
}