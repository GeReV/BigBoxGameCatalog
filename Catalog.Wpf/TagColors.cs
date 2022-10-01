using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Catalog.Wpf
{
    public class TagColors
    {
        private const int DIVISIONS = 6;

        public static readonly IList<Color> All = new List<Color>();

        static TagColors()
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < DIVISIONS; j++)
                {
                    var c = new Cubehelix(
                        j * (360 / DIVISIONS),
                        1f,
                        0.75f - 0.25f * i
                    ).ToColor();

                    All.Add(c);
                }
            }
        }
    }
}
