using System;
using System.IO;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.FileCore {
    public class FileDecodingInputStream : IDecodingInputStream {
        readonly Stream fileStream;
        readonly ByteReader byteReader;
        long streamSize;

        public FileDecodingInputStream(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
        }
        internal FileDecodingInputStream(Stream stream) {
            Guard.IsNotNull(stream, nameof(stream));
            this.streamSize = 0;
            this.byteReader = new ByteReader();
            this.fileStream = stream;
        }

        public void BeginRead() {
            streamSize = fileStream.ReadLong();
        }
        public void EndRead() {
        }
        public bool ReadBit(out Bit bit) {
            if(IsEmpty) {
                bit = Bit.Zero;
                return false;
            }

            if(!byteReader.IsReady) {
                int value = fileStream.ReadByte();
                if(value == -1) {
                    throw new EndOfStreamException();
                }
                byteReader.SetValue((byte)value);
            }
            bit = byteReader.ReadBit();
            streamSize--;
            return true;
        }
        public bool IsEmpty {
            get { return streamSize == 0; }
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}