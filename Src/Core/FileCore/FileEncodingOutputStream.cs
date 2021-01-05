using System;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.FileCore {
    public class FileEncodingOutputStream : IEncodingOutputStream {
        readonly Stream fileStream;
        readonly ByteWriter byteWriter;

        public FileEncodingOutputStream(string fileName, IPlatformService platform) {
            Guard.IsNotNullOrEmpty(fileName, nameof(fileName));
            Guard.IsNotNull(platform, nameof(platform));
            this.byteWriter = new ByteWriter();
            this.fileStream = platform.WriteFile(fileName);
        }
        
        public void BeginWrite() {
        }
        public void EndWrite() {
            if(!byteWriter.IsEmpty)
                fileStream.WriteByte(byteWriter.Value);
        }
        public void WriteBit(Bit bit) {
            byteWriter.AddBit(bit);
            if(byteWriter.IsReady) {
                fileStream.WriteByte(byteWriter.Value);
                byteWriter.Reset();
            }
        }
        public IStreamPosition SavePosition() {
            return new StreamPosition(fileStream.Position);
        }
        public void RestorePosition(IStreamPosition position) {
            Guard.IsNotNull(position, nameof(position));
            fileStream.Position = ((StreamPosition)position).Position;
        }
        public void Dispose() {
            fileStream.Dispose();
        }

        #region StreamPosition

        class StreamPosition : IStreamPosition {
            public StreamPosition(long position) {
                this.Position = position;
            }
            public readonly long Position;
        }

        #endregion
    }
}