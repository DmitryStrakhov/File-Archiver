using System.IO;
using FileArchiver.Base;

namespace FileArchiver.Services {
    public class DefaultPlatformService : IPlatformService {
        public bool FileExists(string path) {
            return File.Exists(path);
        }
        public bool FolderExists(string path) {
            return Directory.Exists(path);
        }
    }
}