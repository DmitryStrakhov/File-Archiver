using System;

namespace FileArchiver.Core.HuffmanCore {
    public interface IEncodingInputStream : IDisposable {
        bool ReadSymbol(out byte symbol);
        void Reset();
    }
}