using System;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.FileCore {
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