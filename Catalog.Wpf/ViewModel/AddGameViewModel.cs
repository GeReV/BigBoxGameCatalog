using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Wpf.Annotations;
using Catalog.Wpf.Commands;
using Condition = Catalog.Model.Condition;

namespace Catalog.Wpf.ViewModel
{
    public sealed class AddGameViewModel : NotifyPropertyChangedBase
    {
        private string gameTitle;
        private string gameMobyGamesSlug;
        private string gameNotes;
        private Platform gamePlatform;
        private DateTime gameReleaseDate;
        private Publisher gamePublisher;
        private int saveProgress;
        private ViewStatus viewStatus = ViewStatus.Idle;
        private ICommand addItemCommand;
        private ICommand removeItemCommand;
        private ItemViewModel currentGameItem;

        public enum ViewStatus
        {
            [Description("Idle")]
            Idle,
            [Description("Downloading screenshots...")]
            DownloadingScreenshots,
        }

        public AddGameViewModel(IEnumerable<Publisher> publishers, IEnumerable<Developer> developers)
        {
            Publishers = new ObservableCollection<Publisher>(publishers);
            Developers = new ObservableCollection<Developer>(developers);

            GameItems = new ObservableCollection<ItemViewModel>
            {
                new ItemViewModel
                {
                    ItemType = Model.ItemTypes.BigBox
                }
            };
            GameLinks = new ObservableCollection<string>();
            GameDevelopers = new ObservableCollection<Developer>();
            GameScreenshots = new ObservableCollection<ScreenshotViewModel>();
            GameTwoLetterIsoLanguageName = new ObservableCollection<string> {"en"};
        }

        public AddGameViewModel() : this(new List<Publisher>(), new List<Developer>())
        {
        }

        public string GameTitle
        {
            get => gameTitle;
            set
            {
                if (value == gameTitle) return;
                gameTitle = value;
                OnPropertyChanged();
            }
        }

        public string GameMobyGamesSlug
        {
            get => gameMobyGamesSlug;
            set
            {
                if (value == gameMobyGamesSlug) return;
                gameMobyGamesSlug = value;
                OnPropertyChanged();
            }
        }

        public string GameNotes
        {
            get => gameNotes;
            set
            {
                if (value == gameNotes) return;
                gameNotes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ItemViewModel> GameItems { get; }

        public ItemViewModel CurrentGameItem
        {
            get => currentGameItem;
            set
            {
                if (Equals(value, currentGameItem)) return;
                currentGameItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> GameLinks { get; }

        public ObservableCollection<string> GameTwoLetterIsoLanguageName { get; }

        public Platform GamePlatform
        {
            get => gamePlatform;
            set
            {
                if (value == gamePlatform) return;
                gamePlatform = value;
                OnPropertyChanged();
            }
        }

        public DateTime GameReleaseDate
        {
            get => gameReleaseDate;
            set
            {
                if (value.Equals(gameReleaseDate)) return;
                gameReleaseDate = value;
                OnPropertyChanged();
            }
        }

        public Publisher GamePublisher
        {
            get => gamePublisher;
            set
            {
                if (Equals(value, gamePublisher)) return;
                gamePublisher = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Developer> GameDevelopers { get; }
        public ObservableCollection<ScreenshotViewModel> GameScreenshots { get; }

        public ObservableCollection<Publisher> Publishers { get; }
        public ObservableCollection<Developer> Developers { get; }

        public IReadOnlyList<Platform> Platforms => Enum
            .GetValues(typeof(Platform))
            .Cast<Platform>()
            .ToList();

        public IReadOnlyList<Condition> Conditions => Enum
            .GetValues(typeof(Condition))
            .Cast<Condition>()
            .ToList();

        public IReadOnlyList<ItemType> ItemTypes => Model.ItemTypes.All.ToList();

        public ViewStatus Status
        {
            get => viewStatus;
            set
            {
                if (value == viewStatus) return;
                viewStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSaving));
            }
        }

        public bool IsSaving => Status != ViewStatus.Idle;

        public int SaveProgress
        {
            get => saveProgress;
            set
            {
                if (value == saveProgress) return;
                saveProgress = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddItemCommand  => addItemCommand ?? (addItemCommand = new AddGameItemCommand(this));
        public ICommand RemoveItemCommand  => removeItemCommand ?? (removeItemCommand = new RemoveGameItemCommand(this));
    }
}