using SkiaSharp;

namespace Catalog.Wpf.Extensions
{
    public static class ColorExtensions
    {
        public static float GetLuminance(this System.Drawing.Color color) =>
            GetLuminance(color.R, color.G, color.B);

        public static float GetLuminance(this System.Windows.Media.Color color) =>
            GetLuminance(color.R, color.G, color.B);

        public static float GetLuminance(this SKColor color) => GetLuminance(color.Red, color.Green, color.Blue);

        private static float GetLuminance(byte r, byte g, byte b) =>
            (0.299f * r + 0.587f * g + 0.114f * b) / 255f;
    }
}
