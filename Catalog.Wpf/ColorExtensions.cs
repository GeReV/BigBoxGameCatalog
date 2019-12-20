using System.Drawing;

namespace Catalog.Wpf
{
    public static class ColorExtensions
    {
        public static float GetLuminance(this Color color) =>
            (0.299f * color.R + 0.587f * color.G + 0.114f * color.B) / 255f;
    }
}
