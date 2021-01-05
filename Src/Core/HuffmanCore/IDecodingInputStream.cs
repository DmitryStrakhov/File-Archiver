using System;
using FileArchiver.Core.DataStructures;

namespace FileArchiver.Core.HuffmanCore {
    public interface IDecodingInputStream : IDisposable {
        bool ReadBit(out Bit bit);
        bool IsEmpty { get; }
    }
}