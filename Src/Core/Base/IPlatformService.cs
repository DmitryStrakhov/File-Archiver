using System;
using System.Collections.Generic;
using System.IO;

namespace FileArchiver.Base {
    public interface IPlatformService {
        bool IsPathExists(string path);
        bool IsFileExists(string path);
        bool IsDirectoryExists(string path);

        void CreateDirectory(string name);
        Stream ReadFile(string path);
        Stream WriteFile(string path);
        IEnumerable<string> EnumFiles(string path);
        IEnumerable<string> EnumDirectories(string path);
    }
}