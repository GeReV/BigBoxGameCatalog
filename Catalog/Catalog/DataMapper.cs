using Catalog.Model;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog
{
    class DataMapper
    {
        static void Initialize()
        {
            var mapper = BsonMapper.Global;

            mapper.Entity<GameCopy>()
                .DbRef(x => x.Developer, "developers")
                .DbRef(x => x.Publisher, "publishers")
                .DbRef(x => x.Media, "media")
                .DbRef(x => x.Appendices, "appendices")
                .Field(x => x.GameBox, "game_box");
        }
    }
}
