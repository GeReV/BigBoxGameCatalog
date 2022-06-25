using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.Commands;
using Catalog.Wpf.Comparers;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ICommand? deleteGameCommand;
        private ICommand? duplicateGameCommand;
        private ICommand? editGameCommand;
        private ListCollectionView filteredGames = new(Array.Empty<GameViewModel>());
        private ObservableCollection<GameViewModel> games = new();
        private ICommand? refreshGames;
        private string? searchTerm;
        private ObservableCollection<GameViewModel> selectedGames = new();
        private ObservableCollection<Tag> tags = new();
        private ICommand? toggleGameTagCommand;
        private MainWindowViewMode viewMode = MainWindowViewMode.GalleryMode;

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

        public ObservableCollection<GameViewModel> SelectedGames
        {
            get => selectedGames;
            set
            {
                if (Equals(value, selectedGames))
                {
                    return;
                }

                selectedGames = value;
                selectedGames.CollectionChanged += SelectedGamesOnCollectionChanged;

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

        public ICommand RefreshGames => refreshGames ??= new DelegateCommand(_ => RefreshGamesCollection());

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

                RefreshGamesCollection();
            }
        );

        public MainWindowViewModel()
        {
            RefreshTags();

            RefreshGamesCollection();

            PropertyChanged += RefreshFilteredGames;

            SelectedGames.CollectionChanged += SelectedGamesOnCollectionChanged;
        }

        private void SelectedGamesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Raise a PropertyChanged event for SelectedGames so the tag context menu updates.
            OnPropertyChanged(nameof(SelectedGames));
        }

        private static IEnumerable<GameCopy> LoadGames(CatalogContext database)
        {
            return database.Games
                .Include(g => g.Items)
                .Include(g => g.GameCopyTags)
                .ThenInclude(t => t.Tag);
        }

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

        public void RefreshGamesCollection()
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

        private void RefreshTags()
        {
            using var database = Application.Current.Database();

            Tags = new ObservableCollection<Tag>(database.Tags.OrderBy(t => t.Name));
        }

        private void RefreshSelectedGames()
        {
            var selectedIds = SelectedGames.Select(game => game.GameCopyId).ToImmutableHashSet();

            SelectedGames =
                new ObservableCollection<GameViewModel>(
                    Games.Where(game => selectedIds.Contains(game.GameCopyId)).ToList()
                );
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
