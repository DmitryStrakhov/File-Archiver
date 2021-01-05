using System;

namespace FileArchiver.Core.HuffmanCore {
    public interface IDecodingOutputStream : IDisposable {
        void WriteSymbol(byte symbol);
    }
}