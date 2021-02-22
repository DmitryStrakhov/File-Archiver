using System;
using FileArchiver.Core.DataStructures;

namespace FileArchiver.Core.HuffmanCore {
    public interface IEncodingOutputStream : IDisposable {
        void BeginWrite();
        void EndWrite();
        void WriteBit(Bit bit);
        long SizeInBytes { get; }
        IStreamPosition SavePosition();
        void RestorePosition(IStreamPosition position);
    }
}