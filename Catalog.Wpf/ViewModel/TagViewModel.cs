using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Catalog.Model;
using Catalog.Wpf.Commands;

namespace Catalog.Wpf.ViewModel
{
    public class TagViewModel : NotifyPropertyChangedBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, ICollection<string>> validationErrors =
            new();

        public TagViewModel(Tag tag)
        {
            Tag = tag;
        }

        public Tag Tag { get; }

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

        #region Model Validation

        public void ValidateModel()
        {
            validationErrors.Clear();

            var validationContext = new ValidationContext(Tag);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            if (Validator.TryValidateObject(Tag, validationContext, validationResults))
            {
                return;
            }

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    if (validationResult.ErrorMessage == null)
                    {
                        continue;
                    }

                    if (!validationErrors.TryGetValue(memberName, out var errors))
                    {
                        errors = new List<string>();

                        validationErrors.Add(memberName, errors);
                    }

                    errors.Add(validationResult.ErrorMessage);
                }
            }

            foreach (var memberName in validationErrors.Keys)
            {
                OnErrorsChanged(memberName);
            }
        }

        private void ValidateModelProperty(object? value, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null)
            {
                return;
            }

            if (validationErrors.ContainsKey(propertyName))
            {
                validationErrors.Remove(propertyName);
            }

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            var validationContext = new ValidationContext(Tag, null, null)
            {
                MemberName = propertyName
            };

            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                var errors = validationResults
                    .Select(validationResult => validationResult.ErrorMessage)
                    .OfType<string>()
                    .ToList();

                validationErrors.Add(propertyName, errors);
            }

            OnErrorsChanged(propertyName);
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !validationErrors.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }

            return validationErrors[propertyName];
        }

        public bool HasErrors => validationErrors.Count > 0;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private void OnErrorsChanged([CallerMemberName] string? propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion
    }
}
