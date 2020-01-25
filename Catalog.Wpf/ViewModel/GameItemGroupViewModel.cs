using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class GameItemGroupViewModel : NotifyPropertyChangedBase
    {
        private ItemType itemType;
        private bool missing;
        private int count;

        public GameItemGroupViewModel(ItemType itemType)
        {
            this.itemType = itemType;
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

        public int Count
        {
            get => count;
            set
            {
                if (value == count) return;
                count = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CountString));
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

        public string CountString => Count > 1 ? $"×{Count}" : string.Empty;
        public string ItemTooltip => ItemType.Description + (Count > 1 ? $" × {Count}" : string.Empty);
    }
}
