using System;
using FileArchiver.Base;
using FileArchiver.Format;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Builders {
    public interface IStreamBuilder {
        void Initialize(IPlatformService platform, EncodingToken token, IEncodingOutputStream stream);
        void AddWeightsTable(BootstrapSegment segment);
        void AddDirectory(DirectorySegment segment);
        void AddFile(FileSegment segment);
    }
}