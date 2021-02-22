using System;
using System.Threading;
using System.Threading.Tasks;
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

        public Task<EncodingStatistics> EncodeAsync(string inputPath, string outputFile, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress) {
            Guard.IsNotNullOrEmpty(inputPath, nameof(inputPath));
            Guard.IsNotNullOrEmpty(outputFile, nameof(outputFile));
            return Task.Run(() => Encode(inputPath, outputFile, cancellationToken, progress), cancellationToken);
        }
        private EncodingStatistics Encode(string inputPath, string outputFile, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress) {
            HuffmanEncoder encoder = new HuffmanEncoder();
            DirectoryEncodingInputStream directoryInputStream = new DirectoryEncodingInputStream(inputPath, fileSystemService, platform);
            FileEncodingOutputStream outputStream = new FileEncodingOutputStream(outputFile, platform);

            try {
                outputStream.BeginWrite();
                ITaskProgressController tpc = CreateTaskProgressController(progress, directoryInputStream);

                tpc.StartIndeterminate();
                EncodingToken encodingToken = encoder.CreateEncodingToken(directoryInputStream, cancellationToken);
                tpc.EndIndeterminate();
                streamBuilder.Initialize(platform, encodingToken, outputStream);
                streamBuilder.AddWeightsTable(new BootstrapSegment(encodingToken.WeightsTable));
                tpc.Start();
                foreach(FileSystemEntry entry in fileSystemService.EnumFileSystemEntries(inputPath)) {
                    switch(entry.Type) {
                        case FileSystemEntryType.Directory:
                            streamBuilder.AddDirectory(new DirectorySegment(entry.Name, entry.Cardinality));
                            break;
                        case FileSystemEntryType.File:
                            streamBuilder.AddFile(new FileSegment(entry.Name, entry.Path), cancellationToken, tpc);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                outputStream.EndWrite();
                tpc.Finish();
                return new EncodingStatistics(directoryInputStream.SizeInBytes, outputStream.SizeInBytes);
            }
            finally {
                directoryInputStream.Dispose();
                outputStream.Dispose();
            }
        }
        private ITaskProgressController CreateTaskProgressController(IProgress<CodingProgressInfo> progress, DirectoryEncodingInputStream inputStream) {
            if(progress == null) return new NullTaskProgressController();
            return new EncodingTaskProgressController(progress, () => inputStream.SizeInBytes);
        }
    }

    
    class EncodingTaskProgressController : ITaskProgressController {
        readonly IProgress<CodingProgressInfo> progress;
        readonly Lazy<long> totalLazy;
        long current;
        IProgressState state;

        public EncodingTaskProgressController(IProgress<CodingProgressInfo> progress, Func<long> getTotal) {
            this.progress = progress;
            this.totalLazy = new Lazy<long>(getTotal);
            this.current = 0;
            this.state = null;
        }

        public void Start() {
            progress.Report(new CodingProgressInfo(0, "[Start]"));
        }
        public void Finish() {
            progress.Report(new CodingProgressInfo(100, "[Finish]"));
        }
        public void StartIndeterminate() {
            progress.Report(new CodingProgressInfo(-1, "[Build encoding token]"));
        }
        public void EndIndeterminate() {
        }
        public void Report(long byteCount, string statusMessage) {
            if(totalLazy.Value == 0) return;
            current += byteCount;
            progress.Report(new CodingProgressInfo((int)(current * 100 / totalLazy.Value), statusMessage));
        }
        IProgressState IProgressHandler.State { get { return state; } set { state = value; } }
    }
}