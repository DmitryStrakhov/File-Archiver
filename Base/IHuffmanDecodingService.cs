using System;

namespace FileArchiver.Base {
    public interface IHuffmanDecodingService {
        bool Decode(string inputFile, string outputFolder);
    }
}