using System;

namespace FileArchiver.Base {
    public interface IPlatformService {
        bool FileExists(string path);
        bool FolderExists(string path);
    }
}