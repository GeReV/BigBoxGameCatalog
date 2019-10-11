using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Catalog.Model;

namespace Catalog.Wpf
{
    public class SelectedDevelopersComparer : IComparer
    {
        private readonly ObservableCollection<Developer> selectedDevelopers;

        public SelectedDevelopersComparer(ObservableCollection<Developer> selectedDevelopers)
        {
            this.selectedDevelopers = selectedDevelopers;
        }

        public int Compare(object x, object y)
        {
            if (!(x is Developer developerX))
            {
                return -1;
            }

            if (!(y is Developer developerY))
            {
                return 1;
            }

            var selectedX = selectedDevelopers.Contains(developerX);
            var selectedY = selectedDevelopers.Contains(developerY);

            if (selectedX != selectedY)
            {
                return selectedX.CompareTo(selectedY);
            }

            return string.Compare(developerX.Name, developerY.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}