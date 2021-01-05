using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Services {
    public class DefaultPlatformService : IPlatformService {
        public DefaultPlatformService() {
        }

        bool IPlatformService.IsPathExists(string path) {
            if(string.IsNullOrEmpty(path)) return false;
            return File.Exists(path) || Directory.Exists(path);
        }
        bool IPlatformService.IsFileExists(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return File.Exists(path);
        }
        bool IPlatformService.IsDirectoryExists(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return Directory.Exists(path);
        }

        void IPlatformService.CreateDirectory(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            Directory.CreateDirectory(path);
        }
        IEnumerable<string> IPlatformService.EnumFiles(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return Directory.GetFiles(path);
        }
        IEnumerable<string> IPlatformService.EnumDirectories(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return Directory.GetDirectories(path);
        }
        Stream IPlatformService.ReadFile(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
        }
        Stream IPlatformService.WriteFile(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        }
    }
}