using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.Builders;
using FileArchiver.Core.FileCore;
using FileArchiver.Core.Format;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Services {
    public class DefaultHuffmanEncodingService : IHuffmanEncodingService {
        readonly IFileSystemService fileSystemService;
        readonly IPlatformService platform;
        readonly IStreamBuilder streamBuilder;

        public DefaultHuffmanEncodingService(IFileSystemService fileSystemService, IPlatformService platform, IStreamBuilder streamBuilder) {
            Guard.IsNotNull(fileSystemService, nameof(fileSystemService));
            Guard.IsNotNull(platform, nameof(platform));
            Guard.IsNotNull(streamBuilder, nameof(streamBuilder));

            this.fileSystemService = fileSystemService;
            this.platform = platform;
            this.streamBuilder = streamBuilder;
        }

        public bool Encode(string inputPath, string outputFile) {
            Guard.IsNotNullOrEmpty(inputPath, nameof(inputPath));
            Guard.IsNotNullOrEmpty(outputFile, nameof(outputFile));

            HuffmanEncoder encoder = new HuffmanEncoder();
            DirectoryEncodingInputStream directoryInputStream = new DirectoryEncodingInputStream(inputPath, fileSystemService, platform);
            FileEncodingOutputStream outputStream = new FileEncodingOutputStream(outputFile, platform);

            try {
                outputStream.BeginWrite();
                EncodingToken encodingToken = encoder.CreateEncodingToken(directoryInputStream);
                streamBuilder.Initialize(platform, encodingToken, outputStream);
                streamBuilder.AddWeightsTable(new BootstrapSegment(encodingToken.WeightsTable));

                foreach(FileSystemEntry entry in fileSystemService.EnumFileSystemEntries(inputPath)) {
                    switch(entry.Type) {
                        case FileSystemEntryType.Directory:
                            streamBuilder.AddDirectory(new DirectorySegment(entry.Name, entry.Cardinality));
                            break;
                        case FileSystemEntryType.File:
                            streamBuilder.AddFile(new FileSegment(entry.Name, entry.Path));
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                outputStream.EndWrite();
            }
            finally {
                directoryInputStream.Dispose();
                outputStream.Dispose();
            }
            return true;
        }
    }
}