using System;

namespace Catalog
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
