using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace Catalog.Wpf.ViewModel
{
    public class TagViewModel : NotifyPropertyChangedBase
    {
        private const int DIVISIONS = 6;

        private string title;
        private Color color;
        private readonly IEnumerable<Color> colors;
        private bool colorPickerIsOpen;

        public TagViewModel()
        {
            var list = new List<Color>();

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < DIVISIONS; j++)
                {
                    var c = new Cubehelix(
                        j * (360 / DIVISIONS),
                        1f,
                        0.75f - 0.25f * i
                    ).ToColor();

                    list.Add(c);
                }
            }

            colors = list;
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

        public Color Color
        {
            get => color;
            set
            {
                if (value.Equals(color)) return;
                color = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Color> Colors => colors;

        public bool ColorPickerIsOpen
        {
            get => colorPickerIsOpen;
            set
            {
                if (value == colorPickerIsOpen) return;
                colorPickerIsOpen = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetColor => new DelegateCommand(param =>
        {
            Color = (Color) param;
            ColorPickerIsOpen = false;
        });
    }
}
