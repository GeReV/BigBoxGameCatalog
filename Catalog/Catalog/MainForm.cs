using System;
using System.Linq;
using Catalog.Model;
using Eto.Drawing;
using Eto.Forms;
using LiteDB;

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

            InsertGame(game);

            UpdateList();
        }

        private static void InsertGame(GameCopy game)
        {
            var database = CatalogApplication.Instance.Database;

            if (game.Publisher.PublisherId == 0)
            {
                database.GetPublishersCollection().Insert(game.Publisher);
            }

            database.GetDevelopersCollection().InsertBulk(game.Developers.Where(d => d.DeveloperId == 0));

            database.GetGamesCollection().Insert(game);
        }

        private void UpdateList()
        {
            var games = CatalogApplication.Instance.Database.GetGamesCollection();

            items.DataStore = games.FindAll();
        }
    }
}
