using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Model
{
    public interface ILocalResource : IEquatable<ILocalResource>
    {
        string Path { get; set; }

        DateTime DateCreated { get; set; }
    }
}
