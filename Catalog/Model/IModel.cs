using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Model
{
    public interface IModel : ITimestamps
    {
        int Id { get; }

        bool IsNew { get; }
    }
}
