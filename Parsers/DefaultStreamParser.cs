using System;
using FileArchiver.Format;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Parsers {
    public class DefaultStreamParser : IStreamParser {
        readonly HuffmanDecoder decoder;

        public DefaultStreamParser() {
            this.decoder = new HuffmanDecoder();
        }

        public BootstrapSegment ParseWeightsTable(IDecodingInputStream stream) {
            Guard.IsNotNull(stream, nameof(stream));

            WeightsTable weightsTable = new WeightsTable();
            int tableSize = stream.ReadInt() / 9;
            for(int n = 0; n < tableSize; n++) {
                byte code = stream.ReadByte();
                long frequency = stream.ReadLong();
                weightsTable.TrackSymbol(code, frequency);
            }
            return new BootstrapSegment(weightsTable);
        }
        public DirectorySegment ParseDirectory(IDecodingInputStream stream) {
            Guard.IsNotNull(stream, nameof(stream));
            int length = stream.ReadInt() / 2;
            string name = ReadString(stream, length);
            int cardinality = stream.ReadInt();
            return new DirectorySegment(name, cardinality);
        }
        public FileSegment ParseFile(IDecodingInputStream stream, WeightsTable weightsTable) {
            Guard.IsNotNull(stream, nameof(stream));
            Guard.IsNotNull(weightsTable, nameof(weightsTable));

            int nameLength = stream.ReadInt() / 2;
            string name = ReadString(stream, nameLength);
            long dataLength = stream.ReadLong();
            return new FileSegment(name, new DefaultFileDecoder(weightsTable, stream, dataLength, decoder));
        }
        static string ReadString(IDecodingInputStream stream, int length) {
            char[] chars = new char[length];

            for(int n = 0; n < length; n++) {
                chars[n] = stream.ReadChar();
            }
            return new string(chars);
        }

        #region DefaultFileDecoder

        class DefaultFileDecoder : IFileDecoder {
            readonly WeightsTable weightsTable;
            readonly IDecodingInputStream inputStream;
            readonly long length;
            readonly HuffmanDecoder decoder;

            public DefaultFileDecoder(WeightsTable weightsTable, IDecodingInputStream inputStream, long length, HuffmanDecoder decoder) {
                this.weightsTable = weightsTable;
                this.inputStream = inputStream;
                this.length = length;
                this.decoder = decoder;
            }

            public void Decode(IDecodingOutputStream outputStream) {
                decoder.Decode(inputStream, weightsTable, outputStream, length);

                int trailingBitCount = MathHelper.ModAdv(length, 8);
                for(int n = 0; n < trailingBitCount; n++) {
                    inputStream.ReadBit(out _);
                }
            }
        }

        #endregion

    }
}