using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.FileCore;
using FileArchiver.Core.Format;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Builders {
    public class DefaultStreamBuilder : IStreamBuilder {
        IPlatformService platform;
        HuffmanEncoder encoder;
        EncodingToken token;
        IEncodingOutputStream stream;

        public DefaultStreamBuilder() {
            this.encoder = null;
        }

        public void Initialize(IPlatformService platform, EncodingToken token, IEncodingOutputStream stream) {
            Guard.IsNotNull(platform, nameof(platform));
            Guard.IsNotNull(token, nameof(token));
            Guard.IsNotNull(stream, nameof(stream));

            this.platform = platform;
            this.token = token;
            this.stream = stream;
            this.encoder = new HuffmanEncoder();
        }
        public void AddWeightsTable(BootstrapSegment segment) {
            Guard.IsNotNull(segment, nameof(segment));

            WeightsTable weightsTable = segment.WeightsTable;
            stream.Write(StreamKind.WT_CODE);
            stream.Write(9 * weightsTable.Size);
            foreach(WeightedSymbol symbol in weightsTable) {
                stream.Write(symbol.Symbol);
                stream.Write(symbol.Weight);
            }
        }
        public void AddDirectory(DirectorySegment segment) {
            Guard.IsNotNull(segment, nameof(segment));

            string name = segment.Name;
            stream.Write(StreamKind.DS_CODE);
            stream.Write(2 * name.Length);
            for(int n = 0; n < name.Length; n++) {
                stream.Write(name[n]);
            }
            stream.Write(segment.Cardinality);
        }
        public void AddFile(FileSegment segment) {
            string name = segment.Name;
            string path = segment.Path;
            Guard.IsNotNullOrEmpty(name, nameof(name));
            Guard.IsNotNullOrEmpty(path, nameof(path));

            stream.Write(StreamKind.FS_CODE);
            stream.Write(2 * name.Length);
            for(int n = 0; n < name.Length; n++) {
                stream.Write(name[n]);
            }
            IStreamPosition sizePosition = stream.SavePosition();
            stream.Write(0L);
            
            using(FileEncodingInputStream fileStream = new FileEncodingInputStream(path, platform)) {
                long sequenceLength = encoder.Encode(fileStream, stream, token);
                EnsureSequenceLayout(sequenceLength);
                IStreamPosition endPosition = stream.SavePosition();
                stream.RestorePosition(sizePosition);
                stream.Write(sequenceLength);
                stream.RestorePosition(endPosition);
            }
        }
        private void EnsureSequenceLayout(long length) {
            int trailingBitCount = MathHelper.ModAdv(length, 8);

            for(int n = 0; n < trailingBitCount; n++) {
                stream.WriteBit(Bit.Zero);
            }
        }
    }
}