using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace App.Helper
{
    public static class FileHelper
    {
        private static readonly List<string> SearchPath_ = new List<string>();

        static FileHelper()
        {
            AddSearchPath(Application.StartupPath);
        }

        public static string FixedPath(string Path)
        {
            Path = Path.Replace('\\', '/');
            if (!Path.EndsWith("/"))
            {
                Path += "/";
            }

            return Path;
        }

        public static string GetDirectory(string Path)
        {
            if (Path.EndsWith("/"))
            {
                return Path;
            }

            var Index = Path.LastIndexOf("/");
            return Path.Substring(0, Index + 1);
        }

        public static void AddSearchPath(string Path)
        {
            SearchPath_.Add(FixedPath(Path));
        }

        public static string Combine(string Path1, string Path2)
        {
            return FixedPath(Path1) + Path2;
        }

        public static bool Exists(string FilePath)
        {
            return File.Exists(FilePath);
        }

        public static string GetFullPath(string FilePath)
        {
            if (Exists(FilePath))
            {
                return FilePath;
            }

            foreach (var Path in SearchPath_)
            {
                var FullPath = Combine(Path, FilePath);
                if (Exists(FullPath))
                {
                    return FullPath;
                }
            }

            return FilePath;
        }
    }
}