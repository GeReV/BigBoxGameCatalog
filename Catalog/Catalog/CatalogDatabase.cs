using Catalog.Model;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog
{
    public class CatalogDatabase : IDisposable
    {
        public LiteDatabase Database { get; private set; }

        public CatalogDatabase(string path)
        {
            Database = new LiteDatabase(path);

            var gameCollection = GetGamesCollection();

            gameCollection.EnsureIndex(x => x.Title);
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        public LiteCollection<GameCopy> GetGamesCollection()
        {
            return Database.GetCollection<GameCopy>("games");
        }
    }
}
