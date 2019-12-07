using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Catalog.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace Catalog
{
    public class CatalogContext : DbContext
    {
        private readonly string databasePath;

        private const char STRING_SEPARATOR = ',';
        private const string GETDATE_SQL = "DATETIME('now')";

        private static readonly ValueConverter<List<string>, string> SplitJoinValueConverter =
            new ValueConverter<List<string>, string>(
                v => string.Join(STRING_SEPARATOR, v),
                v => v.Split(STRING_SEPARATOR, StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        public DbSet<GameCopy> Games { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public CatalogContext() : this("database.sqlite")
        {
        }

        public CatalogContext(string databasePath)
        {
            this.databasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options
                .UseLazyLoadingProxies()
                .UseSqlite($"Data Source={databasePath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildGames(modelBuilder);

            BuildDevelopers(modelBuilder);

            BuildPublishers(modelBuilder);

            BuildGameItems(modelBuilder);

            BuildTags(modelBuilder);

            BuildGameCopyDevelopers(modelBuilder);

            BuildGameCopyTags(modelBuilder);
        }

        private static void BuildGames(ModelBuilder modelBuilder)
        {
            var gameCopyBuilder = modelBuilder.Entity<GameCopy>();

            gameCopyBuilder.HasIndex(v => v.Title);
            gameCopyBuilder.HasIndex(v => v.MobyGamesSlug);

            gameCopyBuilder.Property(v => v.Links).HasConversion(SplitJoinValueConverter);
            gameCopyBuilder.Property(v => v.TwoLetterIsoLanguageName).HasConversion(SplitJoinValueConverter);
            gameCopyBuilder.Property(v => v.Screenshots).HasConversion(SplitJoinValueConverter);
            gameCopyBuilder.Property(v => v.Platforms).HasConversion(
                v => string.Join(STRING_SEPARATOR, v.Select(p => Enum.GetName(typeof(Platform), p))),
                v =>
                    v
                        .Split(STRING_SEPARATOR, StringSplitOptions.RemoveEmptyEntries)
                        .Select(Enum.Parse<Platform>)
                        .ToList()
            );

            ConfigureTimestamps(gameCopyBuilder);
        }

        private static void BuildDevelopers(ModelBuilder modelBuilder)
        {
            var developerBuilder = modelBuilder.Entity<Developer>();

            developerBuilder.HasIndex(v => v.Name).IsUnique();
            developerBuilder.HasIndex(v => v.Slug).IsUnique();
            developerBuilder.Property(v => v.Links).HasConversion(SplitJoinValueConverter);

            ConfigureTimestamps(developerBuilder);
        }

        private static void BuildPublishers(ModelBuilder modelBuilder)
        {
            var publisherBuilder = modelBuilder.Entity<Publisher>();

            publisherBuilder.HasIndex(v => v.Name).IsUnique();
            publisherBuilder.HasIndex(v => v.Slug).IsUnique();
            publisherBuilder.Property(v => v.Links).HasConversion(SplitJoinValueConverter);

            ConfigureTimestamps(publisherBuilder);
        }

        private static void BuildGameItems(ModelBuilder modelBuilder)
        {
            var gameItemBuilder = modelBuilder.Entity<GameItem>();

            gameItemBuilder.Property(v => v.ItemType)
                .HasConversion(
                    v => v.Type,
                    v => ItemTypes.All.First(t => t.Type == v)
                );

            gameItemBuilder
                .HasMany(v => v.Files)
                .WithOne(f => f.GameItem)
                .OnDelete(DeleteBehavior.ClientCascade);

            gameItemBuilder
                .HasMany(v => v.Scans)
                .WithOne(f => f.GameItem)
                .OnDelete(DeleteBehavior.ClientCascade);

            ConfigureTimestamps(gameItemBuilder);
        }

        private void BuildTags(ModelBuilder modelBuilder)
        {
            var tagBuilder = modelBuilder.Entity<Tag>();

            tagBuilder.HasIndex(v => v.Name).IsUnique();
            tagBuilder.Property(v => v.ColorArgb);

            ConfigureTimestamps(tagBuilder);
        }

        private static void BuildGameCopyDevelopers(ModelBuilder modelBuilder)
        {
            var gameCopyDeveloperBuilder = modelBuilder.Entity<GameCopyDeveloper>();

            gameCopyDeveloperBuilder.HasKey(t => new {t.GameCopyId, t.DeveloperId});

            gameCopyDeveloperBuilder
                .HasOne(gd => gd.Developer)
                .WithMany(d => d.GameCopyDevelopers)
                .HasForeignKey(gd => gd.DeveloperId);

            gameCopyDeveloperBuilder
                .HasOne(gd => gd.Game)
                .WithMany(g => g.GameCopyDevelopers)
                .HasForeignKey(gd => gd.GameCopyId);

            ConfigureTimestamps(gameCopyDeveloperBuilder);
        }

        private static void BuildGameCopyTags(ModelBuilder modelBuilder)
        {
            var gameCopyTagBuilder = modelBuilder.Entity<GameCopyTag>();

            gameCopyTagBuilder.HasKey(t => new {t.GameCopyId, t.TagId});

            gameCopyTagBuilder
                .HasOne(gct => gct.Tag)
                .WithMany(d => d.GameCopyTags)
                .HasForeignKey(gd => gd.TagId);

            gameCopyTagBuilder
                .HasOne(gd => gd.Game)
                .WithMany(g => g.GameCopyTags)
                .HasForeignKey(gd => gd.GameCopyId);

            ConfigureTimestamps(gameCopyTagBuilder);
        }

        private static void ConfigureTimestamps<T>(EntityTypeBuilder<T> builder) where T : class, ITimestamps
        {
            builder
                .Property(v => v.DateCreated)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql(GETDATE_SQL);

            builder
                .Property(v => v.LastUpdated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql(GETDATE_SQL);
        }
    }
}
