using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catalog.Model;
using Catalog.Wpf.Commands;

namespace Catalog.Wpf.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        public class Game : NotifyPropertyChangedBase
        {
            private GameCopy gameCopy;

            public Game(GameCopy gameCopy)
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
                    OnPropertyChanged(nameof(Cover));
                }
            }

            public string Title => GameCopy.Title;

            public ImageSource Cover => GameCopy.CoverImage?.Path == null
                ? null
                : new BitmapImage(new Uri(GameCopy.CoverImage.Path));
        }

        private ObservableCollection<Game> games;
        private string searchTerm;
        private ICommand editGameCommand;
        private ICommand deleteGameCommand;

        public MainWindowViewModel()
        {
            RefreshGames = new DelegateCommand(_ => RefreshGamesCollection());

            RefreshGamesCollection();

            FilteredGames = new ListCollectionView(Games)
            {
                CustomSort = new GameComparer(),
                Filter = obj =>
                {
                    if (obj is Game game)
                    {
                        return game.Title.IndexOf(SearchTerm ?? string.Empty,
                                   StringComparison.InvariantCultureIgnoreCase) >= 0;
                    }

                    return false;
                }
            };

            PropertyChanged += RefreshFilteredGames;
        }

        public ListCollectionView FilteredGames { get; }

        public ICommand RefreshGames { get; }

        public ObservableCollection<Game> Games
        {
            get => games;
            set
            {
                if (Equals(value, games)) return;
                games = value;
                OnPropertyChanged();
            }
        }

        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                if (value == searchTerm) return;
                searchTerm = value;
                OnPropertyChanged();
            }
        }

        public ICommand EditGameCommand =>
            editGameCommand ?? (editGameCommand = new EditGameCommand(this));

        public ICommand DeleteGameCommand =>
            deleteGameCommand ?? (deleteGameCommand = new DeleteGameCommand(this));

        public void RefreshGamesCollection()
        {
            var db = Application.Current.Database();

            Games = new ObservableCollection<Game>(
                db.GetGamesCollection().FindAll().Select(gc => new Game(gc))
            );
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