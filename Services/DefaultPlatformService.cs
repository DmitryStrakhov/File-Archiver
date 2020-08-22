using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;

namespace FileArchiver.Services {
    public class DefaultPlatformService : IPlatformService {
        public DefaultPlatformService() {
        }

        bool IPlatformService.IsPathExists(string path) {
            if(string.IsNullOrEmpty(path)) return false;
            return File.Exists(path) || Directory.Exists(path);
        }
        bool IPlatformService.IsFileExists(string path) {
            return File.Exists(path);
        }
        bool IPlatformService.IsDirectoryExists(string path) {
            return Directory.Exists(path);
        }

        IEnumerable<string> IPlatformService.EnumFiles(string path) {
            return Directory.GetFiles(path);
        }
        IEnumerable<string> IPlatformService.EnumDirectories(string path) {
            return Directory.GetDirectories(path);
        }
        Stream IPlatformService.ReadFile(string path) {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
        }
        Stream IPlatformService.WriteFile(string path) {
            return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        }
    }
}