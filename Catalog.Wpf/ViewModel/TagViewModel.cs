using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using Catalog.Model;

namespace Catalog.Wpf.ViewModel
{
    public class TagViewModel : NotifyPropertyChangedBase
    {
        private readonly Tag tag;

        private string title;
        private Color color;

        public TagViewModel(Tag tag)
        {
            this.tag = tag ?? new Tag();

            Title = Tag.Name;
            Color = Tag.Color;
        }

        public Tag Tag => tag;

        public string Title
        {
            get => title;
            set
            {
                if (value == title) return;
                title = value;
                tag.Name = value;
                OnPropertyChanged();
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                if (value.Equals(color)) return;
                color = value;
                tag.Color = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Color> Colors => new ObservableCollection<Color>(TagColors.All);

        public ICommand SetColor => new DelegateCommand(param => { Color = (Color) param; });
    }
}
