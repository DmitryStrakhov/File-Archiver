using System;
using FileArchiver.DataStructures;

namespace FileArchiver.HuffmanCore {
    public interface IDecodingInputStream : IDisposable {
        bool ReadBit(out Bit bit);
        bool IsEmpty { get; }
    }
}