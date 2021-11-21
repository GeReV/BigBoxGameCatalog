using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Model
{
    public interface ILocalResource
    {
        string Path { get; }

        DateTime DateCreated { get; set; }
    }
}
