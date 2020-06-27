using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;

namespace FileArchiver.Services {
    public class DefaultPlatformService : IPlatformService {
        public bool IsPathExists(string path) {
            if(string.IsNullOrEmpty(path)) return false;
            return File.Exists(path) || Directory.Exists(path);
        }
        public bool IsFileExists(string path) {
            return File.Exists(path);
        }
        public bool IsDirectoryExists(string path) {
            return Directory.Exists(path);
        }

        public IEnumerable<string> EnumFiles(string path) {
            return Directory.GetFiles(path);
        }
        public IEnumerable<string> EnumDirectories(string path) {
            return Directory.GetDirectories(path);
        }
    }
}