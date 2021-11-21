using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Model
{
    public sealed class Publisher : IModel
    {
        public Publisher(string name, string slug)
        {
            Slug = slug;
            Name = name;
        }

        public int PublisherId { get; set; }
        [Required] public string Slug { get; set; }
        [Required] public string Name { get; set; }
        public List<string> Links { get; set; } = new();

        public ICollection<GameCopy> Games { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Id => PublisherId;
        public bool IsNew => PublisherId == 0;

        public override string ToString()
        {
            return Name;
        }
    }
}
