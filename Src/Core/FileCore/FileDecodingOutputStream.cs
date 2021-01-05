using System;
using System.IO;
using FileArchiver.Base;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.FileCore {
    public class FileDecodingOutputStream : IDecodingOutputStream {
        readonly Stream fileStream;

        public FileDecodingOutputStream(string fileName, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(fileName, nameof(fileName));
            Guard.IsNotNull(platform, nameof(platform));
            this.fileStream = platform.WriteFile(fileName);
        }

        public void WriteSymbol(byte symbol) {
            fileStream.WriteByte(symbol);
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}