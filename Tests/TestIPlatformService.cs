using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;

namespace FileArchiver.Tests {
    public class TestIPlatformService : IPlatformService {
        public TestIPlatformService() {
        }

        #region IPlatformService

        bool IPlatformService.IsPathExists(string path) => PathExists?.Invoke(path) ?? false;
        bool IPlatformService.IsFileExists(string path) => FileExists?.Invoke(path) ?? false;
        bool IPlatformService.IsDirectoryExists(string path) => DirectoryExists?.Invoke(path) ?? false;

        IEnumerable<string> IPlatformService.EnumFiles(string path) {
            return EnumFilesFunc?.Invoke(path) ?? new string[0];
        }
        IEnumerable<string> IPlatformService.EnumDirectories(string path) {
            return EnumDirectoriesFunc?.Invoke(path) ?? new string[0];
        }
        Stream IPlatformService.ReadFile(string path) {
            return ReadFileFunc?.Invoke(path) ?? throw new InvalidOperationException();
        }
        Stream IPlatformService.WriteFile(string path) {
            return WriteFileFunc?.Invoke(path) ?? throw new InvalidOperationException();
        }

        #endregion

        public Predicate<string> PathExists { get; set; }
        public Predicate<string> FileExists { get; set; }
        public Predicate<string> DirectoryExists { get; set; }

        public Func<string, IEnumerable<string>> EnumFilesFunc { get; set; }
        public Func<string, IEnumerable<string>> EnumDirectoriesFunc { get; set; }
        public Func<string, Stream> ReadFileFunc { get; set; }
        public Func<string, Stream> WriteFileFunc { get; set; }
    }
}