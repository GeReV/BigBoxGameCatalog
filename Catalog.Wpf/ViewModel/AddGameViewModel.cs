using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Catalog.Model;
using Catalog.Wpf.Annotations;
using Condition = Catalog.Model.Condition;

namespace Catalog.Wpf.ViewModel
{
    public sealed class AddGameViewModel : NotifyPropertyChangedBase
    {
        private GameCopy gameCopy;

        public AddGameViewModel(IEnumerable<Publisher> publishers, IEnumerable<Developer> developers)
        {
            GameCopy = new GameCopy();
            Publishers = new ObservableCollection<Publisher>(publishers);
            Developers = new ObservableCollection<Developer>(developers);
            Screenshots = new ObservableCollection<ScreenshotViewModel>();
            FileHashingProgresses = new Dictionary<File, FileViewModel>();
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

        public IDictionary<File, FileViewModel> FileHashingProgresses { get; }

        public IReadOnlyList<Platform> Platforms => Enum
            .GetValues(typeof(Platform))
            .Cast<Platform>()
            .ToList();

        public IReadOnlyList<Condition> Conditions => Enum
            .GetValues(typeof(Condition))
            .Cast<Condition>()
            .ToList();

        public IReadOnlyList<ItemType> ItemTypes => Model.ItemTypes.All.ToList();
    }
}