using System;
using Eto.Forms;
using Eto.Drawing;
using Catalog.Scrapers.MobyGames;
using System.Collections.Generic;

namespace Catalog
{
	public partial class GameDisambiguationDialog : Dialog<GameEntry>
	{
        public List<GameEntry> Entries { get; set; }

        public GameDisambiguationDialog(List<GameEntry> entries)
		{
            Entries = entries;

			InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            List.DataStore = Entries;

            List.ItemKeyBinding = Binding.Property<GameEntry, string>(ge => ge.Href);
            List.ItemTextBinding = Binding.Property<GameEntry, string>(ge => ge.Name);
            List.ItemSubtitleBinding = Binding.Property<GameEntry, string>(ge => string.Join(", ", ge.Releases));

            List.Focus();
        }
    }
}
