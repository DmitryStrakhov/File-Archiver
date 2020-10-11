using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileArchiver.Base;
using FileArchiver.Helpers;

namespace FileArchiver.Services {
    public class DefaultFileSystemService : IFileSystemService {
        readonly IPlatformService platform;

        public DefaultFileSystemService(IPlatformService platform) {
            Guard.IsNotNull(platform, nameof(platform));
            this.platform = platform;
        }

        public IEnumerable<FileSystemEntry> EnumFileSystemEntries(string path) {
            Guard.IsNotNullOrEmpty(path, nameof(path));

            if(platform.IsFileExists(path)) {
                yield return NewFile(path);
            }
            else if(platform.IsDirectoryExists(path)) {
                Queue<string> queue = new Queue<string>(256);
                queue.Enqueue(path);

                while(queue.Count != 0) {
                    string dirPath = queue.Dequeue();
                    string[] subDirs = platform.EnumDirectories(dirPath).ToArray();

                    yield return NewDirectory(dirPath, subDirs.Length);

                    foreach(string filePath in platform.EnumFiles(dirPath)) {
                        yield return NewFile(filePath);
                    }
                    queue.Enqueue(subDirs);
                }
            }
            else throw new ArgumentException(nameof(path));
        }
        private static FileSystemEntry NewFile(string path) {
            string name = Path.GetFileName(path);
            return new FileSystemEntry(FileSystemEntryType.File, name, path);
        }
        private static FileSystemEntry NewDirectory(string path, int cardinality) {
            string name = PathHelper.GetDirectoryName(path);
            return new FileSystemEntry(FileSystemEntryType.Directory, name, path, cardinality);
        }
    }
}