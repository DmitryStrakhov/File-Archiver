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

        public FileDecodingInputStream(string fileName, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(fileName, nameof(fileName));
            Guard.IsNotNull(platform, nameof(platform));

            this.byteReader = new ByteReader();
            this.fileStream = platform.ReadFile(fileName);
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