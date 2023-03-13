using MobyGames.API.DataObjects;

namespace MobyGames.API.Exceptions;

public sealed class MobyGamesApiException : MobyGamesException
{
    public MobyGamesApiException(MobyGamesErrorResponse response) : base(response.Message)
    {
        Code = response.Code;
        Error = response.Error;
    }

    public uint Code { get; }

    public string? Error { get; }
}
