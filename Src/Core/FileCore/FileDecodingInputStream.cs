using System;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.FileCore {
    public class FileDecodingInputStream : IDecodingInputStream {
        readonly Stream fileStream;
        readonly ByteReader byteReader;
        long streamSize;
        long position;

        public FileDecodingInputStream(string path, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            Guard.IsNotNull(platform, nameof(platform));

            this.byteReader = new ByteReader();
            this.position = 0;
            this.fileStream = platform.ReadFile(path);
            this.streamSize = fileStream.Length * 8;
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
            position++;
            return true;
        }
        public long Position {
            get { return position; }
        }
        public long SizeInBytes {
            get { return fileStream.Length; }
        }
        public bool IsEmpty {
            get { return streamSize == 0; }
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}