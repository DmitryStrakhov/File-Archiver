using System.IO;
using System.Linq;

namespace FileArchiver.Helpers {
    public static class PathHelper {
        public static string GetDirectoryName(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));

            if(path.Length <= 3) return path;

            if(path[path.Length - 1] == Path.DirectorySeparatorChar) {
                path = path.Substring(0, path.Length - 1);
            }
            return Path.GetFileName(path);
        }
    }
}