using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.Commands;

namespace Catalog.Wpf.ViewModel
{
    public class TagViewModel : NotifyPropertyChangedBase
    {
        private string? title;
        private Color color;

        public TagViewModel(Tag tag)
        {
            Tag = tag;

            Title = Tag.Name;
            Color = Tag.Color;
        }

        public Tag Tag { get; }

        public string? Title
        {
            get => title;
            set
            {
                if (value == title) return;
                title = value;
                Tag.Name = value;
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
                Tag.Color = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Color> Colors => new ObservableCollection<Color>(TagColors.All);

        public ICommand SetColor => new DelegateCommand(param => { Color = (Color) param; });
    }
}
