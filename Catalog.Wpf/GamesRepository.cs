using System;
using Catalog.Model;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf
{
    public static class GamesRepository
    {
        public static GameCopy? LoadGame(CatalogContext database, int gameCopyId)
        {
            var game = database.Games.Find(gameCopyId);

            if (game == null)
            {
                // TODO: Create an exception for this.
                throw new Exception($"Game with ID {gameCopyId} not found.");
            }

            var entry = database.Entry(game);

            entry
                .Collection(v => v.Items)
                .Query()
                .Include(item => item.Files)
                .Include(item => item.Scans)
                .Load();

            entry
                .Collection(v => v.GameCopyDevelopers)
                .Query()
                .Include(gcd => gcd.Developer)
                .Load();

            entry
                .Reference(v => v.Publisher)
                .Load();

            return game;
        }
    }
}
