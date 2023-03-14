using System.Configuration;

namespace Catalog.Wpf.Helpers;

public static class AppSettingsHelper
{
    public static AppSettingsSection Current =>
        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings;

    public static uint MobyGamesScreenshotLimit(this AppSettingsSection appSettings) =>
        uint.TryParse(appSettings.Settings[nameof(MobyGamesScreenshotLimit)].Value, out var value)
            ? value
            : 10;

    public static string MobyGamesApiKey(this AppSettingsSection appSettings) =>
        appSettings.Settings[nameof(MobyGamesApiKey)].Value;
}
