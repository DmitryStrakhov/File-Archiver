using System;
using System.IO;
using FileArchiver.Base;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.FileCore {
    public class FileDecodingInputStream : IDecodingInputStream {
        readonly Stream fileStream;
        readonly ByteReader byteReader;
        long streamSize;

        public FileDecodingInputStream(string fileName, IPlatformService platform, long streamSize) {
            Guard.IsNotNullOrEmpty(fileName, nameof(fileName));
            Guard.IsNotNull(platform, nameof(platform));
            Guard.IsNotNegative(streamSize, nameof(streamSize));

            this.streamSize = streamSize;
            this.byteReader = new ByteReader();
            this.fileStream = platform.ReadFile(fileName);
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