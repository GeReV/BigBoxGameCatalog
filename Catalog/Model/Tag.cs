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

        public List<GameCopyTag> GameCopyTags { get; set; } = new();

        public List<GameCopy> Games => GameCopyTags.Select(gct => gct.Game).ToList();

        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }

        [NotMapped]
        [Required]
        public Color? Color
        {
            get => ColorArgb == null ? null : System.Drawing.Color.FromArgb(Convert.ToInt32(ColorArgb, 16));
            set => ColorArgb = value.HasValue ? Convert.ToString(value.Value.ToArgb(), 16) : null;
        }

        public int Id => TagId;
        public bool IsNew => TagId == 0;
    }
}
