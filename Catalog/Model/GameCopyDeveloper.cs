namespace Catalog.Model
{
    public class GameCopyDeveloper
    {
        public int GameCopyId { get; set; }
        public GameCopy Game { get; set; }

        public int DeveloperId { get; set; }
        public Developer Developer { get; set; }
    }
}
