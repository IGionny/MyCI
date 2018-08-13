using System;
using System.IO;

namespace MyCI.Common
{
    public static class FileSystemUtilities
    {
        public static void EnsureDirectory(params string[] parameters)
        {
            var dir = Directory.GetCurrentDirectory();
            foreach (var parameter in parameters) dir = Path.Combine(dir, parameter);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}