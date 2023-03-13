namespace MobyGames.API.Exceptions;

public sealed class MobyGamesMissingApiKeyException : Exception
{
    public MobyGamesMissingApiKeyException(string? message) : base(message)
    {
    }
}
