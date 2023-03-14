using System.ComponentModel;

namespace Catalog.Model
{
    public static class ItemTypes
    {
        public static readonly ItemType BigBox = new("bigbox", "Big Box");
        public static readonly ItemType SmallBox = new("smallbox", "Small Box");
        public static readonly ItemType JewelCase = new("jewelcase", "Jewel Case");
        public static readonly ItemType DvdCase = new("dvdcase", "DVD Case");
        public static readonly ItemType Manual = new("manual", "Manual");

        [Category("Media")] public static readonly ItemType Floppy35 = new("floppy35", "3.5\" Floppy");
        [Category("Media")] public static readonly ItemType Floppy525 = new("floppy525", "5.25\" Floppy");
        [Category("Media")] public static readonly ItemType CdRom = new("cdrom", "CD-ROM");
        [Category("Media")] public static readonly ItemType DvdRom = new("dvdrom", "DVD-ROM");
        [Category("Media")] public static readonly ItemType Cassette = new("cassette", "Cassette");

        public static readonly ItemType Appendix = new("appendix", "Appendix");

        public static readonly ItemType[] All =
        {
            BigBox,
            SmallBox,
            JewelCase,
            DvdCase,
            Manual,
            Floppy35,
            Floppy525,
            CdRom,
            DvdRom,
            Cassette,
            Appendix,
        };
    }
}
