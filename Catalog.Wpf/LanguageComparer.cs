using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Catalog.Wpf
{
    public class LanguageComparer : IComparer
    {
        private readonly Func<IEnumerable<CultureInfo>> selectedLanguagesGetter;

        public LanguageComparer(Func<IEnumerable<CultureInfo>> selectedLanguagesGetter)
        {
            this.selectedLanguagesGetter = selectedLanguagesGetter;
        }

        public int Compare(object x, object y)
        {
            if (!(x is CultureInfo ciX))
            {
                return -1;
            }

            if (!(y is CultureInfo ciY))
            {
                return 1;
            }

            var selectedLanguages = selectedLanguagesGetter.Invoke().ToList();

            var compareSelected = CompareFunc(ciX, ciY, ci => selectedLanguages.Contains(ci));

            if (compareSelected != 0)
            {
                return -compareSelected;
            }

            var currentCulture = CompareFunc(ciX, ciY, ci => Equals(ci, CultureInfo.CurrentCulture.Parent));

            if (currentCulture != 0)
            {
                return -currentCulture;
            }

            var currentUiCulture = CompareFunc(ciX, ciY, ci => Equals(ci, CultureInfo.CurrentUICulture.Parent));

            if (currentUiCulture != 0)
            {
                return -currentUiCulture;
            }

            return string.Compare(
                ciX.EnglishName,
                ciY.EnglishName,
                StringComparison.Ordinal
            );
        }

        private int CompareFunc(CultureInfo x, CultureInfo y, Func<CultureInfo, IComparable> func) =>
            func(x).CompareTo(func(y));
    }
}