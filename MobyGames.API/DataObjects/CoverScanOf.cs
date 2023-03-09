using System.ComponentModel;

namespace MobyGames.API.DataObjects;

public enum CoverScanOf
{
    [Description("Front Cover")] FrontCover,
    [Description("Inside Cover")] InsideCover,
    [Description("Back Cover")] BackCover,
    [Description("Media")] Media,
}
