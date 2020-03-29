using System;

namespace FileArchiver.Base {
    public interface IHuffmanEncodingService {
        bool EncodeFile(string inputFile, string outputFile);
        bool EncodeFolder(string inputFolder, string outputFile);
    }
}