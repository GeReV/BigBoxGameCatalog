using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.Commands;
using Catalog.Wpf.Comparers;
using Catalog.Wpf.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ICommand? deleteGameCommand;
        private ICommand? duplicateGameCommand;
        private ICommand? editGameCommand;
        private ICommand? refreshGames;
        private ICommand? toggleGameTagCommand;

        private ObservableCollection<Tag> tags = new();

        private ObservableCollection<GameViewModel> games = new();
        private ListCollectionView filteredGames = new(Array.Empty<GameViewModel>());
        private IList selectedGames = new ArrayList();

        private string? searchTerm;

        private ViewStatus viewStatus = ViewStatus.Loading;
        private MainWindowViewMode viewMode = MainWindowViewMode.GalleryMode;

        private Exception? currentException;

        public enum ViewStatus
        {
            [Description("Loading games...")] Loading,
            Idle,
            Error,
        }

        public ListCollectionView FilteredGames
        {
            get => filteredGames;
            set
            {
                if (Equals(value, filteredGames))
                {
                    return;
                }

                filteredGames = value;
                OnPropertyChanged();
            }
        }

        public IList SelectedGames
        {
            get => selectedGames;
            set
            {
                if (Equals(value, selectedGames))
                {
                    return;
                }

                selectedGames = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GameViewModel> Games
        {
            get => games;
            set
            {
                if (Equals(value, games))
                {
                    return;
                }

                games = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tag> Tags
        {
            get => tags;
            set
            {
                if (Equals(value, tags))
                {
                    return;
                }

                tags = value;
                OnPropertyChanged();
            }
        }

        public string? SearchTerm
        {
            get => searchTerm;
            set
            {
                if (value == searchTerm)
                {
                    return;
                }

                searchTerm = value;
                OnPropertyChanged();
            }
        }

        public ViewStatus Status
        {
            get => viewStatus;
            set
            {
                if (value == viewStatus)
                {
                    return;
                }

                viewStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusDescription));
            }
        }

        public string StatusDescription => Status switch
        {
            ViewStatus.Idle => $"{Games.Count:N0} items",
            ViewStatus.Error => CurrentException == null ? "Unknown Error" : $"Error: {CurrentException.Message}",
            var status => status.GetDescription()
        };

        public MainWindowViewMode ViewMode
        {
            get => viewMode;
            set
            {
                if (Equals(value, viewMode))
                {
                    return;
                }

                viewMode = value;
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

                if (Status == ViewStatus.Error)
                {
                    OnPropertyChanged(nameof(StatusDescription));
                }
            }
        }

        public ICommand RefreshGames => refreshGames ??= new DelegateCommand(_ => RefreshGameCollection());

        public ICommand EditGameCommand =>
            editGameCommand ??= new EditGameCommand(this);

        public ICommand DeleteGameCommand =>
            deleteGameCommand ??= new DeleteGameCommand(this);

        public ICommand DuplicateGameCommand =>
            duplicateGameCommand ??= new DuplicateGameCommand(this);

        public ICommand ToggleGameTagCommand =>
            toggleGameTagCommand ??= new ToggleTagCommand(this);

        public ICommand ChangeViewModeCommand => new DelegateCommand(
            param =>
            {
                if (param is not MainWindowViewMode mode)
                {
                    throw new InvalidOperationException();
                }

                ViewMode = mode;
            }
        );

        public ICommand AddTagCommand => new DelegateCommand(
            param =>
            {
                if (param is not GameViewModel gameViewModel)
                {
                    return;
                }

                var addTagWindow = new EditTagWindow
                {
                    Owner = Application.Current.MainWindow
                };

                if (addTagWindow.ShowDialog() != true)
                {
                    return;
                }

                using var database = Application.Current.Database();

                Debug.Assert(addTagWindow.ResultTag != null, "addTagWindow.ResultTag != null");

                database.Tags.Add(addTagWindow.ResultTag);

                gameViewModel.GameCopy.GameCopyTags.Add(
                    new GameCopyTag
                    {
                        Game = gameViewModel.GameCopy,
                        Tag = addTagWindow.ResultTag
                    }
                );

                database.SaveChanges();

                RefreshTags();

                RefreshGame(gameViewModel.GameCopy.GameCopyId);
            }
        );

        public ICommand CreateTagCommand => new DelegateCommand(
            _ =>
            {
                var addTagWindow = new EditTagWindow
                {
                    Owner = Application.Current.MainWindow
                };

                if (addTagWindow.ShowDialog() != true)
                {
                    return;
                }

                using var database = Application.Current.Database();

                Debug.Assert(addTagWindow.ResultTag != null, "addTagWindow.ResultTag != null");

                database.Tags.Add(addTagWindow.ResultTag);

                database.SaveChanges();

                RefreshTags();
            }
        );

        public ICommand ManageTagsCommand => new DelegateCommand(
            _ =>
            {
                var manageTagsWindow = new ManageTagsWindow
                {
                    Owner = Application.Current.MainWindow
                };

                manageTagsWindow.ShowDialog();

                RefreshTags();

                RefreshGameCollection();
            }
        );

        public MainWindowViewModel()
        {
            PropertyChanged += RefreshFilteredGames;
        }

        private static IQueryable<GameCopy> LoadGames(CatalogContext database) =>
            database.Games
                .Include(g => g.Items)
                .Include(g => g.GameCopyTags)
                .ThenInclude(t => t.Tag);

        public void RefreshGame(int gameCopyId)
        {
            using var database = Application.Current.Database();

            var game = LoadGames(database)
                .SingleOrDefault(g => g.GameCopyId == gameCopyId);

            var existingGame = Games.FirstOrDefault(g => g.GameCopy.GameCopyId == gameCopyId);

            if (existingGame != null)
            {
                Games.Remove(existingGame);
            }

            Games.Add(new GameViewModel(game ?? throw new InvalidOperationException()));
        }

        public void Initialize()
        {
            RefreshTags();

            InitializeGamesCollection();
        }

        private void InitializeGamesCollection()
        {
            using var database = Application.Current.Database();

            var updatedGames = LoadGames(database)
                .Select(gc => new GameViewModel(gc));

            Games = new ObservableCollection<GameViewModel>(updatedGames);

            var updatedFilteredGames = new ListCollectionView(Games)
            {
                CustomSort = new GameComparer(),
                Filter = obj =>
                {
                    if (obj is GameViewModel game)
                    {
                        return game.Title.Contains(
                            SearchTerm ?? string.Empty,
                            StringComparison.InvariantCultureIgnoreCase
                        );
                    }

                    return false;
                }
            };

            updatedFilteredGames.MoveCurrentToPosition(FilteredGames.CurrentPosition);

            FilteredGames = updatedFilteredGames;

            RefreshSelectedGames();
        }

        public void RefreshGameCollection(ISet<int>? gameIds = null)
        {
            using var database = Application.Current.Database();

            var updatedGames = LoadGames(database);
            IEnumerable<GameViewModel> gameViewModels = Games;

            if (gameIds != null)
            {
                updatedGames = updatedGames.Where(gc => gameIds.Contains(gc.GameCopyId));
                gameViewModels = gameViewModels.Where(gc => gameIds.Contains(gc.GameCopyId));
            }

            var updatedGamesDict = updatedGames
                .ToDictionary(gc => gc.GameCopyId);

            foreach (var game in gameViewModels)
            {
                game.GameCopy = updatedGamesDict[game.GameCopyId];
            }

            FilteredGames.Refresh();
        }

        private void RefreshTags()
        {
            using var database = Application.Current.Database();

            Tags = new ObservableCollection<Tag>(database.Tags.OrderBy(t => t.Name));
        }

        private void RefreshSelectedGames()
        {
            var selectedIds = SelectedGames.Cast<GameViewModel>().Select(game => game.GameCopyId).ToImmutableHashSet();

            SelectedGames = Games.Where(game => selectedIds.Contains(game.GameCopyId)).ToList();
        }

        private void RefreshFilteredGames(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SearchTerm))
            {
                return;
            }

            FilteredGames.Refresh();
        }
    }
}
