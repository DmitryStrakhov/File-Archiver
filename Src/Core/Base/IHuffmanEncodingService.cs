using System;

namespace FileArchiver.Base {
    public interface IHuffmanEncodingService {
        bool Encode(string inputPath, string outputFile);
    }
}