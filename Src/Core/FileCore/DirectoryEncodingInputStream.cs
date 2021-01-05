using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.FileCore {
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
            this.fileSystemEntries = new ValueCache<IEnumerator<FileSystemEntry>>(() => this.fileSystemService.EnumFileSystemEntries(this.path).GetEnumerator());
        }
        public bool ReadSymbol(out byte symbol) {
            if(fileStream == null) {
                if(!fileSystemEntries.Value.NextFile()) {
                    symbol = 0;
                    return false;
                }
                fileStream = platform.ReadFile(fileSystemEntries.Value.Current.Path);
            }
            int result = fileStream.ReadByte();
            if(result == -1) {
                fileStream?.Dispose();
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