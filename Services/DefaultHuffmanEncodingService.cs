using System;
using System.Text;
using FileArchiver.Base;
using FileArchiver.FileCore;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Services {
    public class DefaultHuffmanEncodingService : IHuffmanEncodingService {
        readonly IFileSystemService fileSystemService;
        readonly IPlatformService platform;

        public DefaultHuffmanEncodingService(IFileSystemService fileSystemService, IPlatformService platform) {
            Guard.IsNotNull(fileSystemService, nameof(fileSystemService));
            Guard.IsNotNull(platform, nameof(platform));

            this.fileSystemService = fileSystemService;
            this.platform = platform;
        }

        public bool Encode(string inputPath, string outputFile) {
            HuffmanEncoder encoder = new HuffmanEncoder();
            
            DirectoryEncodingInputStream directoryInputStream = new DirectoryEncodingInputStream(inputPath, fileSystemService, platform);
            try {
                EncodingToken encodingToken = encoder.CreateEncodingToken(directoryInputStream);

            }
            finally {
                directoryInputStream.Dispose();
            }
            
            
            //foreach(FileSystemEntry entry in fileSystemService.EnumFileSystemEntries(inputPath)) {
            //}

            //FileEncodingInputStream inputStream = new FileEncodingInputStream(inputPath, platform);
            //FileEncodingOutputStream outputStream = new FileEncodingOutputStream(outputFile, platform);
            //
            //try {
            //    outputStream.BeginWrite();
            //    //new HuffmanEncoder().Encode(inputStream, outputStream, out HuffmanTreeBase tree);
            //    //SharedTree.Instance = tree;
            //    outputStream.EndWrite();
            //}
            //finally {
            //    inputStream.Dispose();
            //    outputStream.Dispose();
            //}
            return true;
        }
    }

    // ToDo
    //
    static class SharedTree {
        public static HuffmanTreeBase Instance { get; set; } = null;
    }
}