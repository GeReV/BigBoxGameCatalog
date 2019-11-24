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
    public class GameItem
    {
        public int GameItemId { get; set; }

        [Required] public ItemType ItemType { get; set; }

        [Required] public GameCopy Game { get; set; }

        public bool Missing { get; set; }

        public Condition? Condition { get; set; }

        public string ConditionDetails { get; set; }

        public string Notes { get; set; }

        public List<Image> Scans { get; set; } = new List<Image>();

        public List<File> Files { get; set; } = new List<File>();
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        [NotMapped]
        public IEnumerable<object> Children => Scans.Concat<object>(Files);

    }
}
