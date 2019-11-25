using System;

namespace Catalog.Model
{
    public interface ITimestamps
    {
        DateTime DateCreated { get; set; }

        DateTime LastUpdated { get; set; }
    }
}
