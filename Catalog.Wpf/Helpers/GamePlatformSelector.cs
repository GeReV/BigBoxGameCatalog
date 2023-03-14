using System;
using System.Linq;
using MobyGames.API.DataObjects;

namespace Catalog.Wpf.Helpers;

public static class GamePlatformSelector
{
    // TODO: Turn into an application-level option?
    private static readonly string[] PlatformPriorities = { "Windows", "DOS" };

    public static GamePlatform SelectPreferredPlatform(Game mobyGame)
    {
        // Find the first release that matches our preferred platforms.
        var result = mobyGame.Platforms[0];
        var index = PlatformPriorities.Length;

        foreach (var gamePlatform in mobyGame.Platforms)
        {
            var i = Array.FindIndex(
                PlatformPriorities,
                p => gamePlatform.Name.Contains(p, StringComparison.InvariantCultureIgnoreCase)
            );

            if (i >= 0 && i < index)
            {
                index = i;
                result = gamePlatform;
            }
        }

        return result;
    }
}
