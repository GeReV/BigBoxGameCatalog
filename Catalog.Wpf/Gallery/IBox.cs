using System.Windows;

namespace Catalog.Wpf.Gallery
{
    public interface IBox : IPaintable
    {
        Thickness Padding { get; init; }
    }
}
