using System;
using System.IO;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.FileCore {
    public class FileDecodingOutputStream : IDecodingOutputStream {
        readonly Stream fileStream;

        public FileDecodingOutputStream(string fileName)
            : this(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
        }
        internal FileDecodingOutputStream(Stream stream) {
            Guard.IsNotNull(stream, nameof(stream));
            this.fileStream = stream;
        }

        public void WriteSymbol(byte symbol) {
            fileStream.WriteByte(symbol);
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}