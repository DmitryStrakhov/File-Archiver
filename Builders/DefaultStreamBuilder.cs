using System;
using FileArchiver.Base;
using FileArchiver.DataStructures;
using FileArchiver.FileCore;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Builders {
    public class DefaultStreamBuilder : IStreamBuilder {
        IPlatformService platform;
        HuffmanEncoder encoder;
        EncodingToken token;
        IEncodingOutputStream stream;

        public DefaultStreamBuilder() {
            this.encoder = null;
        }

        public void Initialize(IPlatformService platform, HuffmanEncoder encoder, EncodingToken token, IEncodingOutputStream stream) {
            Guard.IsNotNull(platform, nameof(platform));
            Guard.IsNotNull(encoder, nameof(encoder));
            Guard.IsNotNull(token, nameof(token));
            Guard.IsNotNull(stream, nameof(stream));

            this.platform = platform;
            this.encoder = encoder;
            this.token = token;
            this.stream = stream;
        }
        public void AddWeightsTable(WeightsTable weightsTable) {
            Guard.IsNotNull(weightsTable, nameof(weightsTable));

            stream.Write(StreamFormat.WT_CODE);
            stream.Write(9 * weightsTable.Size);
            foreach(WeightedSymbol symbol in weightsTable) {
                stream.Write(symbol.Symbol);
                stream.Write(symbol.Weight);
            }
        }
        public void AddDirectory(string name) {
            if(string.IsNullOrEmpty(name)) throw new ArgumentException();

            stream.Write(StreamFormat.DS_CODE);
            stream.Write(2 * name.Length);
            for(int n = 0; n < name.Length; n++) {
                stream.Write(name[n]);
            }
        }
        public void AddFile(string name, string path) {
            Guard.IsNotNullOrEmpty(name, nameof(name));
            Guard.IsNotNullOrEmpty(path, nameof(path));

            stream.Write(StreamFormat.FS_CODE);
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
            int extension = (int)((8L - length % 8) % 8);

            for(int n = 0; n < extension; n++) {
                stream.WriteBit(Bit.Zero);
            }
        }
    }
}