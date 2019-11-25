using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Model
{
    public interface IModel : ITimestamps
    {
        bool IsNew { get; }
    }
}
