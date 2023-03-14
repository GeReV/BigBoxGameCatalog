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
        return mobyGame.Platforms.FirstOrDefault(
            gamePlatform => Array.Exists(
                PlatformPriorities,
                platform => gamePlatform.Name.Contains(platform, StringComparison.InvariantCultureIgnoreCase)
            )
        ) ?? mobyGame.Platforms[0];
    }
}
