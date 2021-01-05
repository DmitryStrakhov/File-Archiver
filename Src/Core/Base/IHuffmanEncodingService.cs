using System;

namespace FileArchiver.Core.Base {
    public interface IHuffmanEncodingService {
        bool Encode(string inputPath, string outputFile);
    }
}