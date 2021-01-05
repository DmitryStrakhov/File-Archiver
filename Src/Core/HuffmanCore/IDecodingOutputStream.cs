using System;

namespace FileArchiver.HuffmanCore {
    public interface IDecodingOutputStream : IDisposable {
        void WriteSymbol(byte symbol);
    }
}