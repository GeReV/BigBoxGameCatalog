using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class ItemViewModel : NotifyPropertyChangedBase
    {
        private ItemType itemType;
        private bool missing;
        private Condition? condition;
        private string conditionDetails;
        private string notes;

        public ItemViewModel()
        {
            Scans = new ObservableCollection<ImageViewModel>();
            Files = new ObservableCollection<FileViewModel>();
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

        public ObservableCollection<ImageViewModel> Scans { get; }

        public ObservableCollection<FileViewModel> Files { get; }

        public Item BuildItem()
        {
            return new Item
            {
                ItemType = ItemType,
                Missing = Missing,
                Condition = Condition,
                ConditionDetails = ConditionDetails,
                Notes = Notes,
                Files = Files.Select(vm => vm.BuildFile()),
                Scans = Scans.Select(vm => vm.BuildImage())
            };
        }
    }
}