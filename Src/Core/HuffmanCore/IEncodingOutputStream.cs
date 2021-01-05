using System;
using FileArchiver.DataStructures;

namespace FileArchiver.HuffmanCore {
    public interface IEncodingOutputStream : IDisposable {
        void BeginWrite();
        void EndWrite();
        void WriteBit(Bit bit);
        IStreamPosition SavePosition();
        void RestorePosition(IStreamPosition position);
    }
}