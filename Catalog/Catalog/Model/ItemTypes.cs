using System.ComponentModel;

namespace Catalog.Model
{
    public static class ItemTypes
    {
        public static ItemType BigBox = new ItemType("bigbox", "Big Box");
        public static ItemType JewelCase = new ItemType("jewelcase", "Jewel Case");
        public static ItemType Manual = new ItemType("manual", "Manual");

        [Category("Media")]
        public static ItemType Floppy35 = new ItemType("floppy35", "3.5\" Floppy");
        [Category("Media")]
        public static ItemType Floppy525 = new ItemType("floppy525", "5.25\" Floppy");
        [Category("Media")]
        public static ItemType CdRom = new ItemType("cdrom", "CD-ROM");
        [Category("Media")]
        public static ItemType DvdRom = new ItemType("dvdrom", "DVD-ROM");

        public static ItemType Cassette = new ItemType("cassette", "Cassette");
        public static ItemType Appendix = new ItemType("appendix", "Appendix");

        public static ItemType[] All = {
            BigBox,
            JewelCase,
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