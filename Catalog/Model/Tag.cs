using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;

namespace Catalog.Model
{
    public class Tag : IModel
    {
        public int TagId { get; set; }

        [Required] public string Name { get; set; }

        public string ColorArgb { get; set; }

        [NotMapped]
        public Color Color
        {
            get => Color.FromArgb(Convert.ToInt32(ColorArgb, 16));
            set => ColorArgb = Convert.ToString(value.ToArgb(), 16);
        }

        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<GameCopyTag> GameCopyTags { get; set; }

        public IEnumerable<GameCopy> Games => GameCopyTags.Select(gct => gct.Game);

        public bool IsNew => TagId == 0;
    }
}
