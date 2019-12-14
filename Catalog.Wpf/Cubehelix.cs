using System;
using System.Drawing;

namespace Catalog.Wpf
{
    public class Cubehelix
    {
        private readonly float h;
        private readonly float s;
        private readonly float l;
        private const float A = -0.14861f;
        private const float B = +1.78277f;
        private const float C = -0.29227f;
        private const float D = -0.90649f;
        private const float E = +1.97294f;

        private const float DEG_2_RAD = (float) (Math.PI / 180f);

        public Cubehelix(float h, float s, float l)
        {
            this.h = h % 360;
            this.s = s;
            this.l = l;
        }

        public Color ToColor()
        {
            var h = (this.h + 120) * DEG_2_RAD;
            var a = s * l * (1 - l);
            var cosh = Math.Cos(h);
            var sinh = Math.Sin(h);

            var r = (int) Math.Clamp(255 * (l + a * (A * cosh + B * sinh)), 0, 255);
            var g = (int) Math.Clamp(255 * (l + a * (C * cosh + D * sinh)), 0, 255);
            var b = (int) Math.Clamp(255 * (l + a * (E * cosh)), 0, 255);

            return Color.FromArgb(r, g, b);
        }
    }
}
