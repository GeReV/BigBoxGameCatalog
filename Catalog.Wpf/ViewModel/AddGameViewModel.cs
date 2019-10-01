using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Catalog.Model;
using Catalog.Wpf.Annotations;

namespace Catalog.Wpf.ViewModel
{
    public sealed class AddGameViewModel : INotifyPropertyChanged
    {
        private GameCopy gameCopy;

        public AddGameViewModel(IEnumerable<Publisher> publishers, IEnumerable<Developer> developers)
        {
            GameCopy = new GameCopy();
            Publishers = new ObservableCollection<Publisher>(publishers);
            Developers = new ObservableCollection<Developer>(developers);
            Screenshots = new ObservableCollection<ScreenshotViewModel>();
        }

        public AddGameViewModel() : this(new List<Publisher>(), new List<Developer>())
        {
        }

        public GameCopy GameCopy
        {
            get => gameCopy;
            set
            {
                if (Equals(value, gameCopy)) return;
                gameCopy = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Publisher> Publishers { get; }
        public ObservableCollection<Developer> Developers { get; }
        public ObservableCollection<ScreenshotViewModel> Screenshots { get; }

        public IReadOnlyList<Platform> Platforms => Enum
            .GetValues(typeof(Platform))
            .Cast<Platform>()
            .ToList();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}