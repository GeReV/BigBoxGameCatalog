namespace MobyGames.API.Exceptions;

public class MobyGamesException : Exception
{
    public MobyGamesException(string? message) : base(message)
    {
    }
}
