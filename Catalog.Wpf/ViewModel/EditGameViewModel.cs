using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Model;
using Catalog.Wpf.Commands;
using Catalog.Wpf.Comparers;
using Catalog.Wpf.Extensions;
using MobyGames.API.DataObjects;
using Condition = Catalog.Model.Condition;
using Platform = Catalog.Model.Platform;

namespace Catalog.Wpf.ViewModel
{
    public sealed class EditGameViewModel : ValidatableViewModelBase
    {
        private readonly GameCopy gameCopy;

        private Game? mobyGame;

        private int saveProgress;
        private ViewStatus viewStatus = ViewStatus.Idle;
        private ItemViewModel? currentGameItem;
        private string? developerSearchTerm;

        private ObservableCollection<CultureInfo> gameLanguages = new() { CultureInfo.GetCultureInfo("en") };

        private ObservableCollection<Developer> gameDevelopers = new();
        private ObservableCollection<Platform> gamePlatforms = new();
        private ObservableCollection<string> gameLinks = new();

        private ObservableCollection<ItemViewModel> gameItems = new()
        {
            new ItemViewModel
            {
                ItemType = Model.ItemTypes.BigBox
            }
        };

        private ObservableCollection<ScreenshotViewModel> gameScreenshots = new();

        private ScreenshotViewModel? gameCoverImage;

        private string? languageSearchTerm;
        private ImageSource? coverImageSource;

        private ICommand? addItemCommand;
        private ICommand? removeItemCommand;
        private ICommand? duplicateItemCommand;
        private ICommand? selectCoverImageCommand;
        private ICommand? removeScreenshotCommand;
        private IAsyncCommand? searchMobyGamesCommand;
        private IAsyncCommand? saveGameCommand;
        private IAsyncCommand? searchMobyGamesCoverCommand;

        private Exception? currentException;

        public enum ViewStatus
        {
            [Description("Idle")] Idle,

            [Description("Downloading screenshots...")]
            DownloadingScreenshots,
            [Description("Searching...")] Searching,
            Error,
        }

        public EditGameViewModel(Window parentWindow, GameCopy gameCopy)
        {
            this.gameCopy = gameCopy;

            ParentWindow = parentWindow;

            using var database = Application.Current.Database();

            Publishers = new ObservableCollection<Publisher>(database.Publishers);
            Developers = new ObservableCollection<Developer>(database.Developers);

            if (!gameCopy.IsNew)
            {
                InitializeData(gameCopy);
            }

            FilteredDevelopers = new ListCollectionView(Developers)
            {
                CustomSort = new SelectedDevelopersComparer(() => GameDevelopers),
                Filter = obj =>
                {
                    if (obj is Developer developer)
                    {
                        return developer.Name.Contains(
                            DeveloperSearchTerm ?? string.Empty,
                            StringComparison.InvariantCultureIgnoreCase
                        );
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
                        return cultureInfo.EnglishName.Contains(
                            LanguageSearchTerm ?? string.Empty,
                            StringComparison.InvariantCultureIgnoreCase
                        );
                    }

                    return false;
                }
            };

            PropertyChanged += RefreshFilteredLanguages;
            PropertyChanged += RefreshFilteredDevelopers;
        }

        private void InitializeData(GameCopy game)
        {
            GameDevelopers = new ObservableCollection<Developer>(game.Developers.Distinct());
            GameLinks = new ObservableCollection<string>(game.Links.Distinct());
            GamePlatforms = new ObservableCollection<Platform>(game.Platforms.Distinct());
            GameLanguages =
                new ObservableCollection<CultureInfo>(
                    game.TwoLetterIsoLanguageName.Distinct().Select(CultureInfo.GetCultureInfo)
                );
            GameItems = new ObservableCollection<ItemViewModel>(game.Items.Select(ItemViewModel.FromItem));
            GameScreenshots = new ObservableCollection<ScreenshotViewModel>(
                game.Screenshots
                    .Select(HomeDirectoryExtensions.ToAbsolutePath)
                    .Select(ScreenshotViewModel.FromPath)
            );
            GameCoverImage = game.CoverImage == null
                ? null
                : ScreenshotViewModel.FromPath(HomeDirectoryExtensions.ToAbsolutePath(game.CoverImage));
        }

        private void RefreshFilteredDevelopers(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(DeveloperSearchTerm) or nameof(Developers) or nameof(GameDevelopers))
            {
                FilteredDevelopers.Refresh();
            }
        }

        private void RefreshFilteredLanguages(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(LanguageSearchTerm) or nameof(GameLanguages))
            {
                FilteredLanguages.Refresh();
            }
        }

        public Window ParentWindow { get; }

        public bool IsNew => gameCopy.IsNew;

        public int GameId
        {
            get => gameCopy.GameCopyId;
            set => gameCopy.GameCopyId = value;
        }

        [Required]
        public string Title
        {
            get => gameCopy.Title;
            set
            {
                if (value == gameCopy.Title) return;
                gameCopy.Title = value;
                OnPropertyChanged();
                ValidateModelProperty(value);
            }
        }

        public bool GameSealed
        {
            get => gameCopy.Sealed;
            set
            {
                if (value == gameCopy.Sealed) return;
                gameCopy.Sealed = value;
                OnPropertyChanged();
            }
        }

        public uint? MobyGameId => gameCopy.MobyGamesId;

        public Game? MobyGame
        {
            get => mobyGame;
            set
            {
                if (value == mobyGame) return;

                mobyGame = value;

                gameCopy.MobyGamesId = value?.Id;
                gameCopy.MobyGamesSlug = value?.MobyUrl.GetComponents(UriComponents.Path, UriFormat.Unescaped)
                    .Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Last();

                OnPropertyChanged();
            }
        }

        public string? GameNotes
        {
            get => gameCopy.Notes;
            set
            {
                if (value == gameCopy.Notes) return;
                gameCopy.Notes = value;
                OnPropertyChanged();
            }
        }

        public DateTime GameReleaseDate
        {
            get => gameCopy.ReleaseDate;
            set
            {
                if (value.Equals(gameCopy.ReleaseDate)) return;
                gameCopy.ReleaseDate = value;
                OnPropertyChanged();
            }
        }

        public Publisher? GamePublisher
        {
            get => gameCopy.Publisher;
            set
            {
                if (Equals(value, gameCopy.Publisher)) return;
                gameCopy.Publisher = value;
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

        public ItemViewModel? CurrentGameItem
        {
            get => currentGameItem;
            set
            {
                if (Equals(value, currentGameItem)) return;
                currentGameItem = value;
                OnPropertyChanged();
            }
        }

        public string? LanguageSearchTerm
        {
            get => languageSearchTerm;
            set
            {
                if (value == languageSearchTerm) return;
                languageSearchTerm = value;
                OnPropertyChanged();
            }
        }

        public Exception? CurrentException
        {
            get => currentException;
            set
            {
                if (Equals(value, currentException)) return;
                currentException = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusDescription));
            }
        }

        public static IEnumerable<CultureInfo> Languages => CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(ci => Equals(ci.Parent, CultureInfo.InvariantCulture) && !Equals(ci, CultureInfo.InvariantCulture));

        public ListCollectionView FilteredLanguages { get; }

        public string? DeveloperSearchTerm
        {
            get => developerSearchTerm;
            set
            {
                if (value == developerSearchTerm) return;
                developerSearchTerm = value;
                OnPropertyChanged();
            }
        }

        public ScreenshotViewModel? GameCoverImage
        {
            get => gameCoverImage;
            set
            {
                if (Equals(value, gameCoverImage)) return;
                gameCoverImage = value;
                OnPropertyChanged();

                coverImageSource = value?.ThumbnailSource;
                OnPropertyChanged(nameof(CoverImageSource));
            }
        }

        public ImageSource? CoverImageSource
        {
            get => coverImageSource;
            set
            {
                if (Equals(value, coverImageSource)) return;
                coverImageSource = value;
                OnPropertyChanged();

                if (value is not BitmapImage bitmapImage)
                {
                    return;
                }

                gameCoverImage = ScreenshotViewModel.FromPath(bitmapImage.UriSource);

                OnPropertyChanged(nameof(GameCoverImage));
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
                OnPropertyChanged(nameof(StatusDescription));
            }
        }

        public string StatusDescription => Status switch
        {
            ViewStatus.Error => CurrentException == null ? "Unknown Error" : $"Error: {CurrentException.Message}",
            var status => status.GetDescription()
        };

        public bool IsSaving => Status == ViewStatus.DownloadingScreenshots;

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
        public ICommand DuplicateItemCommand => duplicateItemCommand ??= new DuplicateGameItemCommand(this);

        public IAsyncCommand SearchMobyGamesCommand =>
            searchMobyGamesCommand ??= new SearchMobyGamesCommand(this);

        public IAsyncCommand SaveGameCommand => saveGameCommand ??= new AsyncDelegateCommand(
            async _ =>
            {
                if (!ValidateModel())
                {
                    return;
                }

                var args = new SaveGameCommand.SaveGameArguments(
                    GameId,
                    this,
                    new Progress<int>(percentage => SaveProgress = percentage)
                );

                try
                {
                    await CommandExecutor.Execute(new SaveGameCommand(), args);

                    // TODO: Ideally this should be external to the viewmodel.
                    ParentWindow.DialogResult = true;

                    ParentWindow.Close();
                }
                catch (Exception)
                {
                    // Ignored. Handled by command.
                }
            }
        );

        public IAsyncCommand SearchMobyGamesCoverCommand =>
            searchMobyGamesCoverCommand ??= new SearchMobyGamesCoverCommand(this);

        public ICommand SelectCoverImageCommand =>
            selectCoverImageCommand ??= new SelectCoverImageCommand(this);

        public ICommand RemoveScreenshotCommand => removeScreenshotCommand ??= new DelegateCommand(
            param =>
            {
                if (param is not IList selectedItems)
                {
                    return;
                }

                var items = selectedItems.Cast<ScreenshotViewModel>().ToList();

                foreach (var selectedItem in items)
                {
                    GameScreenshots.Remove(selectedItem);
                }
            }
        );
    }
}
