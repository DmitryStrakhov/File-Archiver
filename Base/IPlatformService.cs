using System;
using System.Collections.Generic;
using System.IO;

namespace FileArchiver.Base {
    public interface IPlatformService {
        bool IsPathExists(string path);
        bool IsFileExists(string path);
        bool IsDirectoryExists(string path);

        IEnumerable<string> EnumFiles(string path);
        IEnumerable<string> EnumDirectories(string path);
        Stream OpenFile(string path, FileMode fileMode, FileAccess fileAccess);
    }
}