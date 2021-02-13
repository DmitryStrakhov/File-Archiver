using System;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.FileCore {
    public class FileEncodingInputStream : IEncodingInputStream {
        readonly Stream fileStream;
        readonly string path;

        public FileEncodingInputStream(string path, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            Guard.IsNotNull(platform, nameof(platform));

            this.path = path;
            this.fileStream = platform.ReadFile(path);
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
        public string Path {
            get { return path; }
        }
        public void Reset() {
            fileStream.Seek(0, SeekOrigin.Begin);
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}