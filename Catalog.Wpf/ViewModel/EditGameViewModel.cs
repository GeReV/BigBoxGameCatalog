using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Model;
using Catalog.Wpf.Commands;
using Catalog.Wpf.Comparers;
using Condition = Catalog.Model.Condition;

namespace Catalog.Wpf.ViewModel
{
    public sealed class EditGameViewModel : NotifyPropertyChangedBase
    {
        private string gameTitle;
        private string gameMobyGamesSlug;
        private string gameNotes;
        private DateTime gameReleaseDate;
        private Publisher gamePublisher;
        private int saveProgress;
        private ViewStatus viewStatus = ViewStatus.Idle;
        private ItemViewModel currentGameItem;
        private string developerSearchTerm;

        private ObservableCollection<CultureInfo> gameLanguages = new ObservableCollection<CultureInfo>
            {CultureInfo.GetCultureInfo("en")};

        private ObservableCollection<Developer> gameDevelopers = new ObservableCollection<Developer>();
        private ObservableCollection<Platform> gamePlatforms = new ObservableCollection<Platform>();
        private ObservableCollection<string> gameLinks = new ObservableCollection<string>();

        private ObservableCollection<ItemViewModel> gameItems = new ObservableCollection<ItemViewModel>
        {
            new ItemViewModel
            {
                ItemType = Model.ItemTypes.BigBox
            }
        };

        private ScreenshotViewModel gameCoverImage;

        private ObservableCollection<ScreenshotViewModel> gameScreenshots =
            new ObservableCollection<ScreenshotViewModel>();

        private string languageSearchTerm;
        private bool gameSealed;
        private ImageSource coverImageSource;

        private ICommand addItemCommand;
        private ICommand removeItemCommand;
        private ICommand selectCoverImageCommand;
        private ICommand removeScreenshotCommand;
        private IAsyncCommand searchMobyGamesCommand;
        private IAsyncCommand saveGameCommand;
        private IAsyncCommand searchMobyGamesCoverCommand;

        public enum ViewStatus
        {
            [Description("Idle")] Idle,

            [Description("Downloading screenshots...")]
            DownloadingScreenshots,
        }

        public EditGameViewModel(Window parentWindow, GameCopy gameCopy = null)
        {
            ParentWindow = parentWindow;

            using var database = Application.Current.Database();

            Publishers = new ObservableCollection<Publisher>(database.Publishers);
            Developers = new ObservableCollection<Developer>(database.Developers);

            InitializeData(gameCopy);

            FilteredDevelopers = new ListCollectionView(Developers)
            {
                CustomSort = new SelectedDevelopersComparer(() => GameDevelopers),
                Filter = obj =>
                {
                    if (obj is Developer developer)
                    {
                        return developer.Name.IndexOf(DeveloperSearchTerm ?? string.Empty,
                                   StringComparison.InvariantCultureIgnoreCase) >= 0;
                    }

                    return false;
                }
            };

            FilteredLanguages = new ListCollectionView(Languages.ToList())
            {
                CustomSort = new LanguageComparer(() => GameLanguages),
                Filter = obj =>
                {
                    if (obj is CultureInfo cultureInfo)
                    {
                        return cultureInfo.EnglishName.IndexOf(LanguageSearchTerm ?? string.Empty,
                                   StringComparison.InvariantCultureIgnoreCase) >= 0;
                    }

                    return false;
                }
            };

            PropertyChanged += RefreshFilteredLanguages;
            PropertyChanged += RefreshFilteredDevelopers;
        }

        private void InitializeData(GameCopy gameCopy)
        {
            if (gameCopy == null)
            {
                return;
            }

            Game = gameCopy;
            GameTitle = gameCopy.Title;
            GameSealed = gameCopy.Sealed;
            GameMobyGamesSlug = gameCopy.MobyGamesSlug;
            GameNotes = gameCopy.Notes;
            GamePublisher = gameCopy.Publisher;
            GameDevelopers = new ObservableCollection<Developer>(gameCopy.Developers.Distinct());
            GameLinks = new ObservableCollection<string>(gameCopy.Links.Distinct());
            GamePlatforms = new ObservableCollection<Platform>(gameCopy.Platforms.Distinct());
            GameLanguages =
                new ObservableCollection<CultureInfo>(
                    gameCopy.TwoLetterIsoLanguageName.Distinct().Select(lang => CultureInfo.GetCultureInfo(lang)));
            GameItems = new ObservableCollection<ItemViewModel>(gameCopy.Items.Select(ItemViewModel.FromItem));
            GameScreenshots = new ObservableCollection<ScreenshotViewModel>(
                gameCopy.Screenshots.Select(ScreenshotViewModel.FromPath));
            GameCoverImage = gameCopy.CoverImage == null ? null : ScreenshotViewModel.FromPath(gameCopy.CoverImage);
        }

        private void RefreshFilteredDevelopers(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeveloperSearchTerm) || e.PropertyName == nameof(Developers) ||
                e.PropertyName == nameof(GameDevelopers))
            {
                FilteredDevelopers.Refresh();
            }
        }

        private void RefreshFilteredLanguages(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LanguageSearchTerm) || e.PropertyName == nameof(GameLanguages))
            {
                FilteredLanguages.Refresh();
            }
        }

        public Window ParentWindow { get; }

        public GameCopy Game { get; private set; } = new GameCopy();

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

        public bool GameSealed
        {
            get => gameSealed;
            set
            {
                if (value == gameSealed) return;
                gameSealed = value;
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

        public ObservableCollection<ItemViewModel> GameItems
        {
            get => gameItems;
            set
            {
                if (Equals(value, gameItems)) return;
                gameItems = value;
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<string> GameLinks
        {
            get => gameLinks;
            set
            {
                if (Equals(value, gameLinks)) return;
                gameLinks = value;
                OnPropertyChanged();
            }
        }

        public string LanguageSearchTerm
        {
            get => languageSearchTerm;
            set
            {
                if (value == languageSearchTerm) return;
                languageSearchTerm = value;
                OnPropertyChanged();
            }
        }

        public static IEnumerable<CultureInfo> Languages => CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(ci => Equals(ci.Parent, CultureInfo.InvariantCulture) && !Equals(ci, CultureInfo.InvariantCulture));

        public ListCollectionView FilteredLanguages { get; }

        public ObservableCollection<CultureInfo> GameLanguages
        {
            get => gameLanguages;
            set
            {
                if (Equals(value, gameLanguages)) return;
                gameLanguages = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Platform> GamePlatforms
        {
            get => gamePlatforms;
            set
            {
                if (Equals(value, gamePlatforms)) return;
                gamePlatforms = value;
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

        public string DeveloperSearchTerm
        {
            get => developerSearchTerm;
            set
            {
                if (value == developerSearchTerm) return;
                developerSearchTerm = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Developer> GameDevelopers
        {
            get => gameDevelopers;
            set
            {
                if (Equals(value, gameDevelopers)) return;
                gameDevelopers = value;
                OnPropertyChanged();
            }
        }

        public ScreenshotViewModel GameCoverImage
        {
            get => gameCoverImage;
            set
            {
                if (Equals(value, gameCoverImage)) return;
                gameCoverImage = value;
                OnPropertyChanged();

                coverImageSource = value.ThumbnailSource;
                OnPropertyChanged(nameof(CoverImageSource));
            }
        }

        public ImageSource CoverImageSource
        {
            get => coverImageSource;
            set
            {
                if (Equals(value, coverImageSource)) return;
                coverImageSource = value;
                OnPropertyChanged();

                if (!(value is BitmapImage bitmapImage))
                {
                    return;
                }

                gameCoverImage = new ScreenshotViewModel
                {
                    Url = bitmapImage.UriSource.ToString(),
                    ThumbnailUrl = bitmapImage.UriSource.ToString(),
                };

                OnPropertyChanged(nameof(GameCoverImage));
            }
        }

        public ObservableCollection<ScreenshotViewModel> GameScreenshots
        {
            get => gameScreenshots;
            set
            {
                if (Equals(value, gameScreenshots)) return;
                gameScreenshots = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Publisher> Publishers { get; }
        public ObservableCollection<Developer> Developers { get; }
        public ListCollectionView FilteredDevelopers { get; }

        public static IEnumerable<Platform> Platforms => Enum
            .GetValues(typeof(Platform))
            .Cast<Platform>()
            .ToList();

        public static IEnumerable<Condition> Conditions => Enum
            .GetValues(typeof(Condition))
            .Cast<Condition>()
            .ToList();

        public static IEnumerable<ItemType> ItemTypes => Model.ItemTypes.All.ToList();

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

        public ICommand AddItemCommand => addItemCommand ??= new AddGameItemCommand(this);
        public ICommand RemoveItemCommand => removeItemCommand ??= new RemoveGameItemCommand(this);

        public IAsyncCommand SearchMobyGamesCommand =>
            searchMobyGamesCommand ??= new SearchMobyGamesCommand(this);

        public IAsyncCommand SaveGameCommand => saveGameCommand ??= new SaveGameCommand(this);

        public IAsyncCommand SearchMobyGamesCoverCommand =>
            searchMobyGamesCoverCommand ??= new SearchMobyGamesCoverCommand(this);

        public ICommand SelectCoverImageCommand =>
            selectCoverImageCommand ??= new SelectCoverImageCommand(this);

        public ICommand RemoveScreenshotCommand => removeScreenshotCommand ??= new DelegateCommand(param =>
        {
            if (!(param is IList selectedItems))
            {
                return;
            }

            var items = selectedItems.Cast<ScreenshotViewModel>().ToList();

            foreach (var selectedItem in items)
            {
                GameScreenshots.Remove(selectedItem);
            }
        });
    }
}
