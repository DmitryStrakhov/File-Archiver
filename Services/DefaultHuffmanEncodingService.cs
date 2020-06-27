using System;
using System.Text;
using FileArchiver.Base;
using FileArchiver.FileCore;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Services {
    public class DefaultHuffmanEncodingService : IHuffmanEncodingService {
        readonly IFileSystemService fileSystemService;

        public DefaultHuffmanEncodingService(IFileSystemService fileSystemService) {
            Guard.IsNotNull(fileSystemService, nameof(fileSystemService));
            this.fileSystemService = fileSystemService;
        }

        public bool Encode(string inputPath, string outputFile) {
            // ToDo
            foreach(FileSystemEntry entry in fileSystemService.EnumFileSystemEntries(inputPath)) {
            }

            FileEncodingInputStream inputStream = new FileEncodingInputStream(inputPath);
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
    }

    // ToDo
    //
    static class SharedTree {
        public static HuffmanTreeBase Instance { get; set; } = null;
    }
}