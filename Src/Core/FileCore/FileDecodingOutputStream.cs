using System;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.FileCore {
    public class FileDecodingOutputStream : IDecodingOutputStream {
        readonly Stream fileStream;
        readonly string path;

        public FileDecodingOutputStream(string path, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            Guard.IsNotNull(platform, nameof(platform));

            this.path = path;
            this.fileStream = platform.WriteFile(path);
        }

        public void WriteSymbol(byte symbol) {
            fileStream.WriteByte(symbol);
        }
        public string Path {
            get { return path; }
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}