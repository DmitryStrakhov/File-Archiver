using System;
using FileArchiver.Core.DataStructures;

namespace FileArchiver.Core.HuffmanCore {
    public interface IDecodingInputStream : IDisposable {
        bool ReadBit(out Bit bit);
        long Position { get; }
        long SizeInBytes { get; }
        bool IsEmpty { get; }
    }
}