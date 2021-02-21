using System;
using System.Diagnostics;
using System.IO;

namespace FileArchiver.Tests {
    [DebuggerDisplay("TestWritableMemoryStream(Length={Length})")]
    public sealed class WritableMemoryStream : Stream {
        long length;

        public WritableMemoryStream() {
            this.length = 0;
            this.Position = 0;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            length += count;
        }
        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotImplementedException();
        }
        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotImplementedException();
        }
        public override void SetLength(long value) {
            throw new NotImplementedException();
        }
        public override void Flush() {
            throw new NotImplementedException();
        }
        public override long Position { get; set; }
        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override long Length { get { return length; } }
    }
}