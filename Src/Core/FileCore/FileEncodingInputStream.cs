using System;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.FileCore {
    public class FileEncodingInputStream : IEncodingInputStream {
        readonly Stream fileStream;

        public FileEncodingInputStream(string fileName, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(fileName, nameof(fileName));
            Guard.IsNotNull(platform, nameof(platform));

            this.fileStream = platform.ReadFile(fileName);
        }
        
        public bool ReadSymbol(out byte symbol) {
            int result = fileStream.ReadByte();
            if(result == -1) {
                symbol = 0;
                return false;
            }
            symbol = (byte)result;
            return true;
        }
        public void Reset() {
            fileStream.Seek(0, SeekOrigin.Begin);
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}