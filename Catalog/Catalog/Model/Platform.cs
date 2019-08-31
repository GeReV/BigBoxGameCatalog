using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Model
{
    [Flags]
    public enum Platform
    {
        DOS = 1 << 0,
        Win311 = 1 << 1,
        Win95 = 1 << 2,
        Win98 = 1 << 3,
        WinXP = 1 << 4,
        Win7OrHigher = 1 << 5,
    }
}
