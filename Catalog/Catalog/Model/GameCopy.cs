using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Catalog.Model
{
    public class GameCopy : INotifyPropertyChanged
    {
        private List<Item> items;
        private List<Image> screenshots;
        private List<string> links;
        private Platform platform;
        private List<string> twoLetterIsoLanguageName;
        private DateTime releaseDate;
        private Publisher publisher;
        private List<Developer> developers;
        private string mobyGamesSlug;
        private string notes;
        private string title;
        private int gameCopyId;

        public GameCopy()
        {
            Developers = new List<Developer>();
            Links = new List<string>();
            Screenshots = new List<Image>();
            Items = new List<Item>();
            TwoLetterIsoLanguageName = new List<string> {"en"};
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

        public List<Developer> Developers
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

        public List<string> TwoLetterIsoLanguageName
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

        public List<string> Links
        {
            get => links;
            set
            {
                if (Equals(value, links)) return;
                links = value;
                OnPropertyChanged();
            }
        }

        public List<Image> Screenshots
        {
            get => screenshots;
            set
            {
                if (Equals(value, screenshots)) return;
                screenshots = value;
                OnPropertyChanged();
            }
        }

        public List<Item> Items
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