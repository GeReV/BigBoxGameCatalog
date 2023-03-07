using System.ComponentModel;

namespace MobyGames.API.DataObjects;

public enum MobyGamesFormat
{
    [Description("normal")] Normal,

    // TODO: This completely changes the type of the results returned from the API.
    // [Description("id")] Id,

    [Description("brief")] Brief,
}
