using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Catalog.Model
{
    public enum MediaType : byte
    {
        [Description("5.25\" Floppy")]
        Floppy525,
        [Description("3.5\" Floppy")]
        Floppy35,
        [Description("CD-ROM")]
        CdRom,
        [Description("DVD-ROM")]
        DvdRom,
    }
}
