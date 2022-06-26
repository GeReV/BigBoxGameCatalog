using System;

namespace Catalog.Wpf
{
    public static class NumericExtensions
    {
        public static (int, int) DivRem(this int n, int d)
        {
            var div = Math.DivRem(n, d, out var rem);
            
            return (div, rem);
        }
    }
}
