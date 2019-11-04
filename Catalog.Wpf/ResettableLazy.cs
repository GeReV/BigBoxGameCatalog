using System;
using System.Threading;

namespace Catalog.Wpf
{
    public class ResettableLazy<T>
    {
        private Lazy<T> lazy;
        private readonly Func<T> valueFactory;
        private readonly LazyThreadSafetyMode lazyThreadSafetyMode;

        public ResettableLazy(Func<T> valueFactory,
            LazyThreadSafetyMode lazyThreadSafetyMode = LazyThreadSafetyMode.None)
        {
            this.valueFactory = valueFactory;
            this.lazyThreadSafetyMode = lazyThreadSafetyMode;

            lazy = new Lazy<T>(valueFactory, lazyThreadSafetyMode);
        }

        public ResettableLazy(Func<T> valueFactory, bool isThreadSafe) : this(valueFactory,
            isThreadSafe ? LazyThreadSafetyMode.None : LazyThreadSafetyMode.ExecutionAndPublication)
        {
        }

        public T Value => lazy.Value;

        public bool IsValueCreated => lazy.IsValueCreated;

        public void Reset()
        {
            lazy = new Lazy<T>(valueFactory, lazyThreadSafetyMode);
        }
    }
}