using System;

namespace FileArchiver.Core.Base {
    public interface IHuffmanDecodingService {
        void Decode(string inputFile, string outputFolder);
    }
}