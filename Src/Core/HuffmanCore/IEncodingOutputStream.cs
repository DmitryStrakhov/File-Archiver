using System;
using FileArchiver.Core.DataStructures;

namespace FileArchiver.Core.HuffmanCore {
    public interface IEncodingOutputStream : IDisposable {
        void BeginWrite();
        void EndWrite();
        void WriteBit(Bit bit);
        IStreamPosition SavePosition();
        void RestorePosition(IStreamPosition position);
    }
}