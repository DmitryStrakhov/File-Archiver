using System;

namespace FileArchiver.Base {
    public interface IHuffmanDecodingService {
        void Decode(string inputFile, string outputFolder);
    }
}