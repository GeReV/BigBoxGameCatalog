using System.Collections.Generic;
using System.Drawing;

namespace Catalog.Wpf
{
    public class TagColors
    {
        private const int DIVISIONS = 6;

        private static IList<Color> all;

        public static IList<Color> All
        {
            get
            {
                if (all == null)
                {
                    var list = new List<Color>();

                    for (var i = 0; i < 3; i++)
                    {
                        for (var j = 0; j < DIVISIONS; j++)
                        {
                            var c = new Cubehelix(
                                j * (360 / DIVISIONS),
                                1f,
                                0.75f - 0.25f * i
                            ).ToColor();

                            list.Add(c);
                        }
                    }

                    all = list;
                }

                return all;
            }
        }
    }
}
