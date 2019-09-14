using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Catalog.Model
{
    [Flags]
    public enum Platform
    {
        [Description("DOS")]
        Dos = 1 << 0,
        [Description("Windows 3.11")]
        Win311 = 1 << 1,
        [Description("Windows 95")]
        Win95 = 1 << 2,
        [Description("Windows 98")]
        Win98 = 1 << 3,
        [Description("Windows XP")]
        WinXp = 1 << 4,
        [Description("Windows 7 or higher")]
        Win7OrHigher = 1 << 5,
    }
}
