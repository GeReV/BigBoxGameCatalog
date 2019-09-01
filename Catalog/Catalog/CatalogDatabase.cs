using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Model;
using LiteDB;

namespace Catalog
{
    public class CatalogDatabase : IDisposable
    {
        public LiteDatabase Database { get; private set; }

        public CatalogDatabase(string path)
        {
            var mapper = BsonMapper.Global;

            mapper.Entity<GameCopy>()
                .DbRef(x => x.Developers, "developers")
                .DbRef(x => x.Publisher, "publishers")
                .DbRef(x => x.Media, "media")
                .DbRef(x => x.Appendices, "appendices")
                .Field(x => x.GameBox, "game_box");

            Database = new LiteDatabase(path, mapper);

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
