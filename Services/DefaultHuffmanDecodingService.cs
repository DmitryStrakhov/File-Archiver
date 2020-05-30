using System;
using System.IO;
using FileArchiver.Base;
using FileArchiver.FileCore;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Services {
    public class DefaultHuffmanDecodingService : IHuffmanDecodingService {
        public bool Decode(string inputFile, string outputFolder) {
            FileDecodingInputStream inputStream = new FileDecodingInputStream(inputFile);
            FileDecodingOutputStream outputStream = new FileDecodingOutputStream(GetOutputFile(inputFile, outputFolder));

            try {
                inputStream.BeginRead();
                new HuffmanDecoder().Decode(inputStream, SharedTree.Instance.CastTo<HuffmanTree>(), outputStream);
                inputStream.EndRead();
            }
            finally {
                inputStream.Dispose();
                outputStream.Dispose();
            }

            return true;
        }
        private static string GetOutputFile(string inputFile, string outputFolder) {
            string fileName = Path.GetFileNameWithoutExtension(inputFile);
            return Path.Combine(outputFolder, fileName);
        }
    }
}