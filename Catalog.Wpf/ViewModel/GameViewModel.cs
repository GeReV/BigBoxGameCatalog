using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class GameViewModel : NotifyPropertyChangedBase
    {
        private GameCopy gameCopy;
        private ResettableLazy<IEnumerable<GameItemGroupViewModel>> gameStats;

        public GameViewModel()
        {
            gameCopy = new GameCopy();

            gameStats = new ResettableLazy<IEnumerable<GameItemGroupViewModel>>(() =>
                GameItemGrouping.GroupItems(GameCopy.Items));
        }

        public GameViewModel(GameCopy gameCopy) : this()
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

                gameStats.Reset();

                OnPropertyChanged(null);
            }
        }

        public int GameCopyId => GameCopy.GameCopyId;

        public string? Title => GameCopy.Title;

        public Publisher? Publisher => GameCopy.Publisher;

        public IEnumerable<Developer> Developers => GameCopy.Developers;

        public IList<Tag> Tags => GameCopy.Tags;

        public string? Notes => GameCopy.Notes;

        public IEnumerable<GameItem> Items => GameCopy.Items;

        public ImageSource? Cover
        {
            get
            {
                if (GameCopy.CoverImage == null)
                {
                    return null;
                }

                try
                {
                    return new BitmapImage(new Uri(HomeDirectoryHelpers.ToAbsolutePath(GameCopy.CoverImage)));
                }
                catch (IOException)
                {
                    return null;
                }
            }
        }

        public IEnumerable<ScreenshotViewModel> Screenshots =>
            GameCopy.Screenshots
                .Select(HomeDirectoryHelpers.ToAbsolutePath)
                .Select(ScreenshotViewModel.FromPath)
                .ToList();

        public IEnumerable<GameItemGroupViewModel> GameStats => gameStats.Value;

        public bool HasBigBox => GameStats.Any(group => group.ItemType.Equals(ItemTypes.BigBox) && !group.Missing);
    }
}
