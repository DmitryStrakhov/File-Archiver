using System;
using FileArchiver.Base;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Builders {
    public interface IStreamBuilder {
        void Initialize(IPlatformService platform, HuffmanEncoder encoder, EncodingToken token, IEncodingOutputStream stream);
        void AddWeightsTable(WeightsTable weightsTable);
        void AddDirectory(string name);
        void AddFile(string name, string path);
    }
}