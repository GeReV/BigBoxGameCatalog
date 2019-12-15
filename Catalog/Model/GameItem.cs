using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LiteDB;

namespace Catalog.Model
{
    public class GameItem : IModel
    {
        public int GameItemId { get; set; }

        [Required] public ItemType ItemType { get; set; }

        [Required] public virtual GameCopy Game { get; set; }

        public bool Missing { get; set; }

        public Condition? Condition { get; set; }

        public string ConditionDetails { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<Image> Scans { get; set; }

        public virtual ICollection<File> Files { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        [NotMapped]
        public IEnumerable<object> Children => Scans.Concat<object>(Files);

        public int Id => GameItemId;
        public bool IsNew => GameItemId == 0;
    }
}
