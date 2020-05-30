using System;
using FileArchiver.DataStructures;

namespace FileArchiver.HuffmanCore {
    public interface IDecodingInputStream : IDisposable {
        void BeginRead();
        void EndRead();
        bool ReadBit(out Bit bit);
        bool IsEmpty { get; }
    }
}