using System;

namespace FileArchiver.HuffmanCore {
    public interface IEncodingInputStream : IDisposable {
        bool ReadSymbol(out byte symbol);
        void Reset();
    }
}