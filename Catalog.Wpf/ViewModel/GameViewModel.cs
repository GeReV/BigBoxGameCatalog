using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Model;
using Catalog.Wpf.Extensions;

namespace Catalog.Wpf.ViewModel
{
    public class GameViewModel : NotifyPropertyChangedBase
    {
        private readonly ResettableLazy<IEnumerable<GameItemGroupViewModel>> gameStats;
        private GameCopy gameCopy;

        public GameCopy GameCopy
        {
            get => gameCopy;
            set
            {
                if (Equals(value, gameCopy))
                {
                    return;
                }

                gameCopy = value;

                gameStats.Reset();

                OnPropertyChanged(null);
            }
        }

        public int GameCopyId => GameCopy.GameCopyId;

        public string Title => GameCopy.Title;

        public bool IsSealed => GameCopy.Sealed;

        public Publisher? Publisher => GameCopy.Publisher;

        public IEnumerable<Developer> Developers => GameCopy.Developers;

        public IEnumerable<Tag> Tags => GameCopy.Tags;

        public IEnumerable<string> Links => GameCopy.Links;

        public string? Notes => GameCopy.Notes;

        public IEnumerable<GameItem> Items => GameCopy.Items;

        public string? CoverPath =>
            GameCopy.CoverImage != null ? HomeDirectoryExtensions.ToAbsolutePath(GameCopy.CoverImage) : null;

        public ImageSource? Cover
        {
            get
            {
                if (CoverPath == null)
                {
                    return null;
                }

                try
                {
                    return new BitmapImage(new Uri(CoverPath));
                }
                catch (IOException)
                {
                    return null;
                }
            }
        }

        public IEnumerable<ScreenshotViewModel> Screenshots =>
            GameCopy.Screenshots
                .Select(HomeDirectoryExtensions.ToAbsolutePath)
                .Select(ScreenshotViewModel.FromPath)
                .ToList();

        public IEnumerable<GameItemGroupViewModel> GameStats => gameStats.Value;

        public bool HasBigBox => GameStats.Any(group => group.ItemType.Equals(ItemTypes.BigBox) && !group.Missing);

        public GameViewModel()
        {
            gameCopy = new GameCopy();

            gameStats = new ResettableLazy<IEnumerable<GameItemGroupViewModel>>(
                () =>
                    GameItemGrouping.GroupItems(GameCopy.Items)
            );
        }

        public GameViewModel(GameCopy gameCopy) : this()
        {
            GameCopy = gameCopy;
        }
    }
}
