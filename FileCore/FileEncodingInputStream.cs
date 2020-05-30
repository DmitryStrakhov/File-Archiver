using System;
using System.IO;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.FileCore {
    public class FileEncodingInputStream : IEncodingInputStream {
        readonly Stream fileStream;

        public FileEncodingInputStream(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
        }
        internal FileEncodingInputStream(Stream stream) {
            Guard.IsNotNull(stream, nameof(stream));
            this.fileStream = stream;
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