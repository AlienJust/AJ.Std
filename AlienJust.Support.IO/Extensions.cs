using System;
using System.IO;

namespace AlienJust.Support.IO
{
    public static class Extensions
    {
        public static string GetRelativeToDirectoryPath(this string directoryPath, string someAbsolutePath)
        {
            var uri1 = new Uri(Path.GetFullPath(someAbsolutePath));
            var uri2 = new Uri(Path.GetFullPath(directoryPath) + Path.DirectorySeparatorChar);
            return Uri.UnescapeDataString(uri2.MakeRelativeUri(uri1).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static string GetRelativeToFilePath(this string filePath, string someAbsolutePath)
        {
            var uri1 = new Uri(Path.GetFullPath(someAbsolutePath));
            var uri2 = new Uri(Path.GetFullPath(filePath));
            return Uri.UnescapeDataString(uri2.MakeRelativeUri(uri1).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static string GetAbsolutePath(this string basePath, string someRelativeOrAbsolutePath)
        {
            return Path.GetFullPath(Path.Combine(basePath, someRelativeOrAbsolutePath));
        }
    }
}