using System.IO;
using System.Windows;

namespace Catalog.Wpf.Extensions
{
    public static class HomeDirectoryExtensions
    {
        public static string ToAbsolutePath(string relativePath) =>
            Path.GetFullPath(relativePath, Application.Current.HomeDirectory());

        public static string ToRelativePath(string fullPath) =>
            Path.GetRelativePath(Application.Current.HomeDirectory(), fullPath);
    }
}
