using System;
using System.Collections.Generic;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog
{
    public partial class GameDisambiguationDialog : Dialog<SearchResult>
    {
        public List<SearchResult> Entries { get; set; }

        public GameDisambiguationDialog(List<SearchResult> entries)
        {
            Entries = entries;

            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            List.DataStore = Entries;

            List.ItemKeyBinding = Binding.Property<SearchResult, string>(ge => ge.Url);
            List.ItemTextBinding = Binding.Property<SearchResult, string>(ge => ge.Name);
            List.ItemSubtitleBinding = Binding.Property<SearchResult, string>(ge => string.Join(", ", ge.Releases));

            List.Focus();
        }
    }
}
