using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Extensions;
using MobyGames.API.DataObjects;
using MobyGames.API.Helpers;

namespace MobyGames.API;

public interface IMobyClientOptions
{
    public void Serialize(QueryBuilder queryBuilder);
}

public static class MobyGamesClientOptions
{
    public class CommonOptions : IMobyClientOptions
    {
        [Description("The maximum number of groups to return")]
        [Range(1, 100)]
        [DefaultValue(100)]
        public uint Limit { get; set; } = 100;

        [Description("The offset from which to begin returning groups")]
        [DefaultValue(0)]
        public uint Offset { get; set; }

        public virtual void Serialize(QueryBuilder queryBuilder)
        {
            if (!this.IsDefault(nameof(Limit)))
            {
                queryBuilder.Add("limit", Limit.ToString());
            }

            if (!this.IsDefault(nameof(Offset)))
            {
                queryBuilder.Add("offset", Offset.ToString());
            }
        }
    }

    public sealed class GamesOptions : CommonOptions
    {
        [Description("A substring of the title (not case sensitive)")]
        public string? Title { get; set; }

        [Description("The output format: id, brief, or normal")]
        [DefaultValue(MobyGamesFormat.Normal)]
        public MobyGamesFormat? Format { get; set; }

        [Description("IDs of games to return. If specified, other parameters besides format will be ignored")]
        public IEnumerable<uint> Ids { get; set; } = Enumerable.Empty<uint>();

        [Description("The ID of a platform on which the game was released")]
        public IEnumerable<uint> Platforms { get; set; } = Enumerable.Empty<uint>();

        [Description("The ID of a genre assigned to the game")]
        public IEnumerable<uint> Genres { get; set; } = Enumerable.Empty<uint>();

        [Description("The ID of a group assigned to the game")]
        public IEnumerable<uint> Groups { get; set; } = Enumerable.Empty<uint>();

        public override void Serialize(QueryBuilder queryBuilder)
        {
            base.Serialize(queryBuilder);

            if (!string.IsNullOrEmpty(Title))
            {
                queryBuilder.Add("title", Title);
            }

            if (Format is not null && !this.IsDefault(nameof(Format)))
            {
                queryBuilder.Add("format", Format.GetDescription());
            }

            foreach (var id in Ids)
            {
                queryBuilder.Add("id", id.ToString());
            }

            foreach (var platform in Platforms)
            {
                queryBuilder.Add("platform", platform.ToString());
            }

            foreach (var genre in Genres)
            {
                queryBuilder.Add("genre", genre.ToString());
            }

            foreach (var group in Groups)
            {
                queryBuilder.Add("group", group.ToString());
            }
        }
    }

    public sealed class RecentGamesOptions : CommonOptions
    {
        [Description("Return only games modified in the last `age` days")]
        [Range(1, 21)]
        [DefaultValue(21)]
        public uint Age { get; set; }

        // [Description("The output format: id, brief, or normal")]
        // [DefaultValue(MobyGamesFormat.Id)]
        // public MobyGamesFormat? Format { get; set; }

        public override void Serialize(QueryBuilder queryBuilder)
        {
            base.Serialize(queryBuilder);

            if (!this.IsDefault(nameof(Age)))
            {
                queryBuilder.Add("age", Age.ToString());
            }

            // if (Format is not null && !this.IsDefault(nameof(Format)))
            // {
            //     queryBuilder.Add("format", Format.GetDescription());
            // }
        }
    }

    public sealed class RandomGamesOptions : IMobyClientOptions
    {
        [Description("The maximum number of games to return")]
        [Range(1, 100)]
        [DefaultValue(100)]
        public uint Limit { get; set; }

        // [Description("The output format: id, brief, or normal")]
        // [DefaultValue(MobyGamesFormat.Id)]
        // public MobyGamesFormat? Format { get; set; }

        public void Serialize(QueryBuilder queryBuilder)
        {
            if (!this.IsDefault(nameof(Limit)))
            {
                queryBuilder.Add("limit", Limit.ToString());
            }

            // if (Format is not null && !this.IsDefault(nameof(Format)))
            // {
            //     queryBuilder.Add("format", Format.GetDescription());
            // }
        }
    }

    public sealed class GameOptions : IMobyClientOptions
    {
        [Description("The output format: id, brief, or normal")]
        [DefaultValue(MobyGamesFormat.Normal)]
        public MobyGamesFormat? Format { get; set; }

        public void Serialize(QueryBuilder queryBuilder)
        {
            if (Format is not null && !this.IsDefault(nameof(Format)))
            {
                queryBuilder.Add("format", Format.GetDescription());
            }
        }
    }
}
