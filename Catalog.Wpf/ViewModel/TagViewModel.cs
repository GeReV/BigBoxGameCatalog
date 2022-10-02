using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.Commands;

namespace Catalog.Wpf.ViewModel
{
    public class TagViewModel : ValidatableViewModelBase
    {
        public TagViewModel(Tag tag)
        {
            Tag = tag;
        }

        public Tag Tag { get; }

        [Required]
        public string Name
        {
            get => Tag.Name;
            set
            {
                if (value == Tag.Name) return;

                Tag.Name = value;

                OnPropertyChanged();
                ValidateModelProperty(value);
            }
        }

        [Required]
        public Color? Color
        {
            get => Tag.Color;
            set
            {
                if (value.Equals(Tag.Color)) return;

                Tag.Color = value;

                OnPropertyChanged();
                ValidateModelProperty(value);
            }
        }

        public static IReadOnlyCollection<Color> Colors => new ReadOnlyCollection<Color>(TagColors.All);

        public ICommand SetColor => new DelegateCommand(
            param =>
            {
                if (param is not Color c)
                {
                    throw new InvalidOperationException();
                }

                Color = c;
            }
        );
    }
}
