using System;
using System.Collections.Generic;

namespace FileArchiver.Base {
    public interface IPlatformService {
        bool IsPathExists(string path);
        bool IsFileExists(string path);
        bool IsDirectoryExists(string path);
        IEnumerable<string> EnumFiles(string path);
        IEnumerable<string> EnumDirectories(string path);
    }
}