﻿using System;
using System.Collections.Generic;
using System.Linq;
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

            mapper.UseLowerCaseDelimiter();

            mapper.RegisterType(
                type => type.Type,
                value => ItemTypes.All.First(it => it.Type == value.AsString)
            );

            mapper.Entity<GameCopy>()
                .DbRef(x => x.Developers, "developers")
                .DbRef(x => x.Publisher, "publishers");

            mapper.Entity<Publisher>().Id(x => x.PublisherId);

            mapper.Entity<Developer>().Id(x => x.DeveloperId);

            Database = new LiteDatabase(path, mapper);

            var gameCollection = GetGamesCollection();

            gameCollection.EnsureIndex(x => x.Title);

            var publisherCollection = GetPublishersCollection();

            publisherCollection.EnsureIndex(x => x.Name, true);

            var developerCollection = GetDevelopersCollection();

            developerCollection.EnsureIndex(x => x.Name, true);
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        public LiteCollection<GameCopy> GetGamesCollection()
        {
            return Database.GetCollection<GameCopy>("games");
        }

        public LiteCollection<Publisher> GetPublishersCollection()
        {
            return Database.GetCollection<Publisher>("publishers");
        }

        public LiteCollection<Developer> GetDevelopersCollection()
        {
            return Database.GetCollection<Developer>("developers");
        }
    }
}