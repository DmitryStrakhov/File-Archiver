using System;
using FileArchiver.Base;
using FileArchiver.FileCore;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Services {
    public class DefaultHuffmanEncodingService : IHuffmanEncodingService {
        public bool EncodeFile(string inputFile, string outputFile) {
            FileEncodingInputStream inputStream = new FileEncodingInputStream(inputFile);
            FileEncodingOutputStream outputStream = new FileEncodingOutputStream(outputFile);

            try {
                outputStream.BeginWrite();
                new HuffmanEncoder().Encode(inputStream, outputStream, out HuffmanTreeBase tree);
                SharedTree.Instance = tree;
                outputStream.EndWrite();
            }
            finally {
                inputStream.Dispose();
                outputStream.Dispose();
            }
            return true;
        }
        public bool EncodeFolder(string inputFolder, string outputFile) {
            throw new NotImplementedException();
        }
    }

    // ToDo
    //
    static class SharedTree {
        public static HuffmanTreeBase Instance { get; set; } = null;
    }
}