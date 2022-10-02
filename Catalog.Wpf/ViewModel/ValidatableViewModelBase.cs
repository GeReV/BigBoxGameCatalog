using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Catalog.Wpf.ViewModel
{
    public class ValidatableViewModelBase : NotifyPropertyChangedBase, IValidatable
    {
        #region Model Validation

        private readonly Dictionary<string, ICollection<string>> validationErrors =
            new();

        protected virtual object ValidationModel => this;

        public bool ValidateModel()
        {
            validationErrors.Clear();

            var validationContext = new ValidationContext(this);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            if (Validator.TryValidateObject(ValidationModel, validationContext, validationResults))
            {
                return true;
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

            return false;
        }

        protected void ValidateModelProperty(object? value, [CallerMemberName] string? propertyName = null)
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

            var validationContext = new ValidationContext(ValidationModel, null, null)
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
