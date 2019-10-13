using System;
using System.Collections.Generic;
using System.Linq;

namespace Catalog
{
    public class AggregateProgress<T>
    {
        private Action<IEnumerable<T>> action;

        private readonly Dictionary<Progress<T>, T> dictionary = new Dictionary<Progress<T>, T>();

        public AggregateProgress() {}
        public AggregateProgress(Action<IEnumerable<T>> action)
        {
            this.action = action;
        }

        public AggregateProgress(IEnumerable<Progress<T>> progresses, Action<IEnumerable<T>> action) : this(action)
        {
            foreach (var progress in progresses)
            {
                progress.ProgressChanged += ProgressOnProgressChanged;

                dictionary.Add(progress, default(T));
            }
        }


        public void Add(Progress<T> progress)
        {
            if (dictionary.ContainsKey(progress))
            {
                return;
            }

            progress.ProgressChanged += ProgressOnProgressChanged;

            dictionary.Add(progress, default(T));
        }

        private void ProgressOnProgressChanged(object sender, T e)
        {
            dictionary[(Progress<T>) sender] = e;

            OnProgressChanged();
        }

        public event EventHandler<IEnumerable<T>> ProgressChanged;

        protected virtual void OnProgressChanged()
        {
            var progressValues = dictionary.Values.ToList();

            ProgressChanged?.Invoke(this, progressValues);

            action?.Invoke(progressValues);
        }
    }
}