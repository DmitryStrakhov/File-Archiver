using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.FileCore {
    public class DirectoryEncodingInputStream : IEncodingInputStream {
        readonly IFileSystemService fileSystemService;
        readonly IPlatformService platform;
        readonly string path;
        readonly ValueCache<IEnumerator<FileSystemEntry>> fileSystemEntries;
        Stream fileStream;

        public DirectoryEncodingInputStream(string path, IFileSystemService fileSystemService, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            Guard.IsNotNull(fileSystemService, nameof(fileSystemService));
            Guard.IsNotNull(platform, nameof(platform));

            this.path = path;
            this.fileSystemService = fileSystemService;
            this.platform = platform;
            this.fileStream = null;
            this.fileSystemEntries = new ValueCache<IEnumerator<FileSystemEntry>>(() => fileSystemService.EnumFileSystemEntries(this.path).GetEnumerator());
        }
        public bool ReadSymbol(out byte symbol) {
            if(fileStream == null) {
                if(!fileSystemEntries.Value.NextFile()) {
                    symbol = 0;
                    return false;
                }
                fileStream = platform.OpenFile(fileSystemEntries.Value.Current.Path, FileMode.Open, FileAccess.Read);
            }
            int result = fileStream.ReadByte();
            if(result == -1) {
                fileStream = null;
                return ReadSymbol(out symbol);
            }
            symbol = (byte)result;
            return true;
        }
        public void Reset() {
            fileStream?.Dispose();
            fileStream = null;
            fileSystemEntries.Reset();
        }
        public void Dispose() {
            fileStream?.Dispose();
        }
    }
}