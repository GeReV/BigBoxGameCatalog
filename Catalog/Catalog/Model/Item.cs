using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Catalog.Model
{
    public class Item : NotifyPropertyChangedBase
    {
        private ItemType itemType;
        private bool missing;
        private Condition? condition;
        private string conditionDetails;
        private string notes;
        private ObservableCollection<Image> scans;
        private ObservableCollection<File> files;

        public Item()
        {
            scans = new ObservableCollection<Image>();
            files = new ObservableCollection<File>();
        }

        public ItemType ItemType
        {
            get => itemType;
            set
            {
                if (Equals(value, itemType)) return;
                itemType = value;
                OnPropertyChanged();
            }
        }

        public bool Missing
        {
            get => missing;
            set
            {
                if (value == missing) return;
                missing = value;
                OnPropertyChanged();
            }
        }

        public Condition? Condition
        {
            get => condition;
            set
            {
                if (value == condition) return;
                condition = value;
                OnPropertyChanged();
            }
        }

        public string ConditionDetails
        {
            get => conditionDetails;
            set
            {
                if (value == conditionDetails) return;
                conditionDetails = value;
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

        public ObservableCollection<Image> Scans => scans;

        public ObservableCollection<File> Files => files;
    }
}