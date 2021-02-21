using System;
using System.Threading;
using FileArchiver.Core.Base;
using FileArchiver.Core.Format;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Builders {
    public interface IStreamBuilder {
        void Initialize(IPlatformService platform, EncodingToken encodingToken, IEncodingOutputStream stream);
        void AddWeightsTable(BootstrapSegment segment);
        void AddDirectory(DirectorySegment segment);
        void AddFile(FileSegment segment, CancellationToken cancellationToken, IProgressHandler progress);
    }
}