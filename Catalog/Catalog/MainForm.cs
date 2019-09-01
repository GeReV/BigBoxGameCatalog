using System;
using Catalog.Model;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            items.ItemTextBinding = Binding.Property<GameCopy, string>(g => g.Title);

            UpdateList();
        }

        private void AddGame(GameCopy game)
        {
            if (game == null)
            {
                return;
            }

            var collection = CatalogApplication.Instance.Database.GetGamesCollection();

            collection.Insert(game);

            UpdateList();
        }

        private void UpdateList()
        {
            var games = CatalogApplication.Instance.Database.GetGamesCollection();

            items.DataStore = games.FindAll();
        }
    }
}
