﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.Commands;
using Catalog.Wpf.Comparers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Catalog.Wpf.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ObservableCollection<Tag> tags = new ObservableCollection<Tag>();
        private ObservableCollection<GameViewModel> games = new ObservableCollection<GameViewModel>();
        private IList selectedGames = new ArrayList();
        private ListCollectionView filteredGames = new ListCollectionView(new ArrayList());
        private string? searchTerm;
        private MainWindowViewMode viewMode = MainWindowViewMode.GalleryMode;
        private ICommand? editGameCommand;
        private ICommand? deleteGameCommand;
        private ICommand? duplicateGameCommand;
        private ICommand? toggleGameTagCommand;
        private ICommand? refreshGames;

        public MainWindowViewModel()
        {
            RefreshTags();

            RefreshGamesCollection();

            PropertyChanged += RefreshFilteredGames;
        }

        public ListCollectionView FilteredGames
        {
            get => filteredGames;
            set
            {
                if (Equals(value, filteredGames)) return;
                filteredGames = value;
                OnPropertyChanged();
            }
        }

        public IList SelectedGames
        {
            get => selectedGames;
            set
            {
                if (Equals(value, selectedGames)) return;
                selectedGames = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GameViewModel> Games
        {
            get => games;
            set
            {
                if (Equals(value, games)) return;
                games = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tag> Tags
        {
            get => tags;
            set
            {
                if (Equals(value, tags)) return;
                tags = value;
                OnPropertyChanged();
            }
        }

        public string? SearchTerm
        {
            get => searchTerm;
            set
            {
                if (value == searchTerm) return;
                searchTerm = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewMode ViewMode
        {
            get => viewMode;
            set
            {
                if (Equals(value, viewMode)) return;
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

        public ICommand ChangeViewModeCommand => new DelegateCommand(mode => { ViewMode = (MainWindowViewMode) mode; });

        public ICommand AddTagCommand => new DelegateCommand((param) =>
        {
            if (!(param is GameViewModel gameViewModel))
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

            database.Tags.Add(addTagWindow.ResultTag);

            gameViewModel.GameCopy.GameCopyTags.Add(new GameCopyTag
            {
                Game = gameViewModel.GameCopy,
                Tag = addTagWindow.ResultTag
            });

            database.SaveChanges();

            RefreshTags();

            RefreshGame(gameViewModel.GameCopy.GameCopyId);
        });

        public ICommand CreateTagCommand => new DelegateCommand(_ =>
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

            database.Tags.Add(addTagWindow.ResultTag);

            database.SaveChanges();

            RefreshTags();
        });

        public ICommand ManageTagsCommand => new DelegateCommand(_ =>
        {
            var manageTagsWindow = new ManageTagsWindow
            {
                Owner = Application.Current.MainWindow
            };

            manageTagsWindow.ShowDialog();

            RefreshTags();

            RefreshGamesCollection();
        });

        private static IEnumerable<GameCopy> LoadGames(CatalogContext database) =>
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

            Games.Add(new GameViewModel(game));
        }

        public void RefreshGamesCollection()
        {
            using var database = Application.Current.Database();

            var updatedGames = LoadGames(database)
                .Select(gc => new GameViewModel(gc));

            // var selectedItems = SelectedGames
            //     .Cast<GameViewModel>()
            //     .Select(game => game.GameCopy.GameCopyId)
            //     .ToList();

            Games = new ObservableCollection<GameViewModel>(updatedGames);

            FilteredGames = new ListCollectionView(Games)
            {
                CustomSort = new GameComparer(),
                Filter = obj =>
                {
                    if (obj is GameViewModel game)
                    {
                        return game.Title?.IndexOf(SearchTerm ?? string.Empty,
                                   StringComparison.InvariantCultureIgnoreCase) >= 0;
                    }

                    return false;
                }
            };

            // TODO: Figure out how to get command to not lock after an update.
            // SelectedGames = new ArrayList(Games
            //     .Where(game => selectedItems.Contains(game.GameCopy.GameCopyId))
            //     .ToList());
        }

        private void RefreshTags()
        {
            using var database = Application.Current.Database();

            Tags = new ObservableCollection<Tag>(database.Tags.OrderBy(t => t.Name));
        }

        private void RefreshFilteredGames(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SearchTerm))
            {
                return;
            }

            FilteredGames.Refresh();
        }
    }
}
