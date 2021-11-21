using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;

namespace Catalog.Model
{
    public class Tag : IModel
    {
        public Tag() : this(string.Empty)
        {
        }

        public Tag(string name, string? colorArgb = null)
        {
            Name = name;
            ColorArgb = colorArgb;
        }

        public int TagId { get; set; }

        [Required] public string Name { get; set; }

        public string? ColorArgb { get; set; }

        [NotMapped]
        public Color Color
        {
            get => Color.FromArgb(Convert.ToInt32(ColorArgb, 16));
            set => ColorArgb = Convert.ToString(value.ToArgb(), 16);
        }

        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }

        public ICollection<GameCopyTag> GameCopyTags { get; set; }

        public IEnumerable<GameCopy> Games => GameCopyTags.Select(gct => gct.Game);

        public int Id => TagId;
        public bool IsNew => TagId == 0;
    }
}
