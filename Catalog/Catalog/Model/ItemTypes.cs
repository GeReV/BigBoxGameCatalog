using System.ComponentModel;

namespace Catalog.Model
{
    public static class ItemTypes
    {
        public static ItemType BigBox = new ItemType("bigbox", "Big Box"/*, new ImageSource(Icons.box)*/);
        public static ItemType JewelCase = new ItemType("jewelcase", "Jewel Case"/*, new Bitmap(Icons.disc_case) */);
        public static ItemType Manual = new ItemType("manual", "Manual"/*, new Bitmap(Icons.book) */);

        [Category("Media")]
        public static ItemType Floppy35 = new ItemType("floppy35", "3.5\" Floppy"/*, new Bitmap(Icons.disk) */);
        [Category("Media")]
        public static ItemType Floppy525 = new ItemType("floppy525", "5.25\" Floppy"/*, new Bitmap(Icons.disk_black) */);
        [Category("Media")]
        public static ItemType CdRom = new ItemType("cdrom", "CD-ROM"/*, new Bitmap(Icons.disc) */);
        [Category("Media")]
        public static ItemType DvdRom = new ItemType("dvdrom", "DVD-ROM"/*, new Bitmap(Icons.disc_blue) */);

        public static ItemType Cassette = new ItemType("cassette", "Cassette"/*, new Bitmap(Icons.cassette) */);
        public static ItemType Appendix = new ItemType("appendix", "Appendix"/*, new Bitmap(Icons.document) */);

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