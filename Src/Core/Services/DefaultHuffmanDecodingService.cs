using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.FileCore;
using FileArchiver.Core.Format;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.Parsers;

namespace FileArchiver.Core.Services {
    public class DefaultHuffmanDecodingService : IHuffmanDecodingService {
        readonly IPlatformService platform;
        readonly IStreamParser streamParser;
        readonly Queue<string> directoriesQueue;
        string currentDirectory;

        public DefaultHuffmanDecodingService(IPlatformService platform, IStreamParser streamParser) {
            Guard.IsNotNull(platform, nameof(platform));
            Guard.IsNotNull(streamParser, nameof(streamParser));

            this.platform = platform;
            this.streamParser = streamParser;
            this.directoriesQueue = new Queue<string>(128);
        }

        public void Decode(string inputFile, string outputFolder) {
            Guard.IsNotNullOrEmpty(inputFile, nameof(inputFile));
            Guard.IsNotNullOrEmpty(outputFolder, nameof(outputFolder));

            FileDecodingInputStream inputStream = new FileDecodingInputStream(inputFile, platform);

            try {
                if(inputStream.IsEmpty)
                    return;

                StreamKind code = inputStream.ReadStreamFormat();
                if(code != StreamKind.WT_CODE)
                    throw new InvalidOperationException();

                directoriesQueue.Enqueue(currentDirectory = outputFolder);
                BootstrapSegment bootstrapSegment = streamParser.ParseWeightsTable(inputStream);
                while(!inputStream.IsEmpty) {
                    switch(inputStream.ReadStreamFormat()) {
                        case StreamKind.FS_CODE:
                            DecodeFile(inputStream, bootstrapSegment);
                            break;
                        case StreamKind.DS_CODE:
                            DecodeDirectory(inputStream);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
            finally {
                inputStream.Dispose();
            }
        }
        private void DecodeFile(FileDecodingInputStream inputStream, BootstrapSegment bootstrapSegment) {
            FileSegment file = streamParser.ParseFile(inputStream, bootstrapSegment.WeightsTable);
            string path = Path.Combine(currentDirectory, file.Name);

            using(FileDecodingOutputStream outputStream = new FileDecodingOutputStream(path, platform)) {
                file.FileDecoder.Decode(outputStream);
            }
        }
        private void DecodeDirectory(FileDecodingInputStream inputStream) {
            DirectorySegment directory = streamParser.ParseDirectory(inputStream);
            UpdateCurrentDirectory(directory);
            platform.CreateDirectory(currentDirectory);
        }
        private void UpdateCurrentDirectory(DirectorySegment directory) {
            if(directoriesQueue.Count == 0) {
                throw new InvalidOperationException();
            }
            currentDirectory = Path.Combine(directoriesQueue.Dequeue(), directory.Name);

            for(int n = 0; n < directory.Cardinality; n++) {
                directoriesQueue.Enqueue(currentDirectory);
            }
        }
    }
}