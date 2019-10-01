using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Catalog.Model
{
    public class GameCopy : INotifyPropertyChanged
    {
        private ObservableCollection<Item> items;
        private ObservableCollection<Image> screenshots;
        private ObservableCollection<string> links;
        private Platform platform;
        private ObservableCollection<string> twoLetterIsoLanguageName;
        private DateTime releaseDate;
        private Publisher publisher;
        private ObservableCollection<Developer> developers;
        private string mobyGamesSlug;
        private string notes;
        private string title;
        private int gameCopyId;

        public GameCopy()
        {
            Developers = new ObservableCollection<Developer>();
            Links = new ObservableCollection<string>();
            Screenshots = new ObservableCollection<Image>();
            Items = new ObservableCollection<Item>();
            TwoLetterIsoLanguageName = new ObservableCollection<string> {"en"};
        }

        public int GameCopyId
        {
            get => gameCopyId;
            set
            {
                if (value == gameCopyId) return;
                gameCopyId = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (value == title) return;
                title = value;
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get => notes;
            set
            {
                if (value == notes) return;
                notes = value;
                OnPropertyChanged();
            }
        }

        public string MobyGamesSlug
        {
            get => mobyGamesSlug;
            set
            {
                if (value == mobyGamesSlug) return;
                mobyGamesSlug = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Developer> Developers
        {
            get => developers;
            set
            {
                if (Equals(value, developers)) return;
                developers = value;
                OnPropertyChanged();
            }
        }

        public Publisher Publisher
        {
            get => publisher;
            set
            {
                if (Equals(value, publisher)) return;
                publisher = value;
                OnPropertyChanged();
            }
        }

        public DateTime ReleaseDate
        {
            get => releaseDate;
            set
            {
                if (value.Equals(releaseDate)) return;
                releaseDate = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> TwoLetterIsoLanguageName
        {
            get => twoLetterIsoLanguageName;
            set
            {
                if (Equals(value, twoLetterIsoLanguageName)) return;
                twoLetterIsoLanguageName = value;
                OnPropertyChanged();
            }
        }

        public Platform Platform
        {
            get => platform;
            set
            {
                if (value == platform) return;
                platform = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Links
        {
            get => links;
            set
            {
                if (Equals(value, links)) return;
                links = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Image> Screenshots
        {
            get => screenshots;
            set
            {
                if (Equals(value, screenshots)) return;
                screenshots = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Item> Items
        {
            get => items;
            set
            {
                if (Equals(value, items)) return;
                items = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}