using System;
using System.IO;
using System.Text;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.FileCore {
    public class FileEncodingOutputStream : IEncodingOutputStream {
        readonly Stream fileStream;
        readonly ByteWriter byteWriter;
        long streamSize;

        public FileEncodingOutputStream(string fileName)
            : this(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
        }
        internal FileEncodingOutputStream(Stream stream) {
            Guard.IsNotNull(stream, nameof(stream));
            this.streamSize = 0;
            this.fileStream = stream;
            this.byteWriter = new ByteWriter();
        }

        public void BeginWrite() {
            fileStream.Seek(sizeof(long), SeekOrigin.Begin);
        }
        public void EndWrite() {
            if(!byteWriter.IsEmpty) {
                fileStream.WriteByte(byteWriter.Value);
            }
            
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.WriteLong(streamSize);
            fileStream.Close();
        }
        public void WriteBit(Bit bit) {
            streamSize++;
            byteWriter.AddBit(bit);
            if(byteWriter.IsReady) {
                fileStream.WriteByte(byteWriter.Value);
                byteWriter.Reset();
            }
        }
        public void Dispose() {
            fileStream.Dispose();
        }
    }
}