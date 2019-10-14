using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ObservableCollection<GameCopy> games;

        public MainWindowViewModel()
        {
            RefreshGames = new DelegateCommand(_ => RefreshGamesCollection());

            RefreshGamesCollection();
        }

        public ICommand RefreshGames { get; }

        public ObservableCollection<GameCopy> Games
        {
            get => games;
            set
            {
                if (Equals(value, games)) return;
                games = value;
                OnPropertyChanged();
            }
        }

        private void RefreshGamesCollection()
        {
            var db = Application.Current.Database();

            Games = new ObservableCollection<GameCopy>(db.GetGamesCollection().FindAll());
        }
    }
}