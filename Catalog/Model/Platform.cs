using System.ComponentModel;

namespace Catalog.Model
{
    public enum Platform
    {
        [Description("DOS")] Dos = 1,
        [Description("Windows")] Windows,
        [Description("Windows 3.11")] Win311,
        [Description("Windows 95")] Win95,
        [Description("Windows 98")] Win98,
        [Description("Windows XP")] WinXp,
        [Description("Windows 7 or higher")] Win7OrHigher,
    }
}
