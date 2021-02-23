using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileArchiver.Core.Base;
using FileArchiver.Core.FileCore;
using FileArchiver.Core.Format;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;
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

        public Task DecodeAsync(string inputFile, string outputFolder, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress) {
            Guard.IsNotNullOrEmpty(inputFile, nameof(inputFile));
            Guard.IsNotNullOrEmpty(outputFolder, nameof(outputFolder));
            return Task.Run(() => Decode(inputFile, outputFolder, cancellationToken, progress), cancellationToken);
        }
        private void Decode(string inputFile, string outputFolder, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress) {
            FileDecodingInputStream inputStream = new FileDecodingInputStream(inputFile, platform);

            ITaskProgressController tpc = CreateTaskProgressController(progress, inputStream);
            try {
                if(inputStream.IsEmpty)
                    return;

                StreamKind code = inputStream.ReadStreamFormat();
                if(code != StreamKind.WT_CODE)
                    throw new InvalidOperationException();

                tpc.Start();
                directoriesQueue.Enqueue(currentDirectory = outputFolder);
                BootstrapSegment bootstrapSegment = streamParser.ParseWeightsTable(inputStream);
                while(!inputStream.IsEmpty) {
                    switch(inputStream.ReadStreamFormat()) {
                        case StreamKind.FS_CODE:
                            DecodeFile(inputStream, bootstrapSegment, cancellationToken, tpc);
                            break;
                        case StreamKind.DS_CODE:
                            DecodeDirectory(inputStream);
                            break;
                        default:
                            throw new StreamFormatException();
                    }
                }
                tpc.Finish();
            }
            catch {
                tpc.Error();
                throw;
            }
            finally {
                inputStream.Dispose();
            }
        }
        private void DecodeFile(FileDecodingInputStream inputStream, BootstrapSegment bootstrapSegment, CancellationToken cancellationToken, IProgressHandler progressHandler) {
            FileSegment file = streamParser.ParseFile(inputStream, bootstrapSegment.WeightsTable);
            string path = Path.Combine(currentDirectory, file.Name);

            using(FileDecodingOutputStream outputStream = new FileDecodingOutputStream(path, platform)) {
                file.FileDecoder.Decode(outputStream, cancellationToken, progressHandler);
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
        private ITaskProgressController CreateTaskProgressController(IProgress<CodingProgressInfo> progress, FileDecodingInputStream inputStream) {
            if(progress == null) return new NullTaskProgressController();
            return new DecodingTaskProgressController(progress, inputStream.SizeInBytes);
        }
    }

    
    class DecodingTaskProgressController : ITaskProgressController {
        readonly IProgress<CodingProgressInfo> progress;
        readonly long total;
        long current;
        IProgressState state;

        public DecodingTaskProgressController(IProgress<CodingProgressInfo> progress, long total) {
            this.progress = progress;
            this.total = total;
            this.current = 0;
            this.state = null;
        }
        public void Start() {
            progress.Report(new CodingProgressInfo(0, "[Start]"));
        }
        public void Finish() {
            progress.Report(new CodingProgressInfo(100, "[Finish]"));
        }
        public void Error() {
            progress.Report(new CodingProgressInfo(0, "[Decoding Error]"));
        }
        public void StartIndeterminate() {
        }
        public void EndIndeterminate() {
        }
        public void Report(long byteCount, string statusMessage) {
            if(total == 0) return;
            current += byteCount;
            progress.Report(new CodingProgressInfo((int)(current * 100 / total), statusMessage));
        }
        IProgressState IProgressHandler.State { get { return state; } set { state = value; } }
    }
}