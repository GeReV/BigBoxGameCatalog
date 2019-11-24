using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog
{
    public class CatalogContext : DbContext
    {
        private const char STRING_SEPARATOR = ',';

        private readonly ValueConverter<List<string>, string> splitJoinValueConverter = new ValueConverter<List<string>, string>(
            v => string.Join(STRING_SEPARATOR, v),
            v => v.Split(STRING_SEPARATOR, StringSplitOptions.RemoveEmptyEntries).ToList()
        );

        public DbSet<GameCopy> Games { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlite($"Data Source=database.sqlite");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var gameCopyBuilder = modelBuilder.Entity<GameCopy>();

            gameCopyBuilder.HasIndex(v => v.Title);
            gameCopyBuilder.HasIndex(v => v.MobyGamesSlug);

            gameCopyBuilder.Property(v => v.Links).HasConversion(splitJoinValueConverter);
            gameCopyBuilder.Property(v => v.TwoLetterIsoLanguageName).HasConversion(splitJoinValueConverter);
            gameCopyBuilder.Property(v => v.Platforms).HasConversion(
                v => string.Join(STRING_SEPARATOR, v.Select(p => Enum.GetName(typeof(Platform), p))),
                v =>
                    v
                        .Split(STRING_SEPARATOR, StringSplitOptions.RemoveEmptyEntries)
                        .Select(Enum.Parse<Platform>)
                        .ToList()
            );

            var developerBuilder = modelBuilder.Entity<Developer>();

            developerBuilder.HasIndex(v => v.Name).IsUnique();
            developerBuilder.HasIndex(v => v.Slug).IsUnique();
            developerBuilder.Property(v => v.Links).HasConversion(splitJoinValueConverter);

            var publisherBuilder = modelBuilder.Entity<Publisher>();

            publisherBuilder.HasIndex(v => v.Name).IsUnique();
            publisherBuilder.HasIndex(v => v.Slug).IsUnique();
            publisherBuilder.Property(v => v.Links).HasConversion(splitJoinValueConverter);

            modelBuilder.Entity<GameItem>()
                .Property(v => v.ItemType)
                .HasConversion(
                    v => v.Type,
                    v => ItemTypes.All.First(t => t.Type == v)
                );

            var gameCopyDeveloperBuilder = modelBuilder.Entity<GameCopyDeveloper>();

            gameCopyDeveloperBuilder.HasKey(t => new { t.GameCopyId, t.DeveloperId });

            gameCopyDeveloperBuilder
                .HasOne(gd => gd.Developer)
                .WithMany(d => d.GameCopyDevelopers)
                .HasForeignKey(gd => gd.DeveloperId);

            gameCopyDeveloperBuilder
                .HasOne(gd => gd.Game)
                .WithMany(g => g.GameCopyDevelopers)
                .HasForeignKey(gd => gd.GameCopyId);
        }
    }
}
