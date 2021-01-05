using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.Format;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Builders {
    public interface IStreamBuilder {
        void Initialize(IPlatformService platform, EncodingToken token, IEncodingOutputStream stream);
        void AddWeightsTable(BootstrapSegment segment);
        void AddDirectory(DirectorySegment segment);
        void AddFile(FileSegment segment);
    }
}