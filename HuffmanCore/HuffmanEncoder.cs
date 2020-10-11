using System;
using System.Collections.Generic;
using System.Diagnostics;
using FileArchiver.Helpers;
using FileArchiver.DataStructures;

namespace FileArchiver.HuffmanCore {
    public class EncodingToken {
        public EncodingToken(WeightsTable weightsTable, HuffmanTreeBase huffmanTree, CodingTable codingTable) {
            CodingTable = codingTable;
            WeightsTable = weightsTable;
            HuffmanTree = huffmanTree;
        }
        public CodingTable CodingTable { get; }
        public HuffmanTreeBase HuffmanTree { get; }
        public WeightsTable WeightsTable { get; }
    }

    
    public class HuffmanEncoder {
        public HuffmanEncoder() {
        }

        public EncodingToken CreateEncodingToken(IEncodingInputStream inputStream) {
            Guard.IsNotNull(inputStream, nameof(inputStream));

            WeightsTable weightsTable = EncodingTools.BuildWeightsTable(inputStream);
            HuffmanTreeBase huffmanTree = EncodingTools.BuildHuffmanTree(weightsTable);
            CodingTable codingTable = EncodingTools.BuildCodingTable(huffmanTree);
            return new EncodingToken(weightsTable, huffmanTree, codingTable);
        }
        
        public long Encode(IEncodingInputStream inputStream, IEncodingOutputStream outputStream, EncodingToken encodingToken) {
            Guard.IsNotNull(inputStream, nameof(inputStream));
            Guard.IsNotNull(outputStream, nameof(outputStream));
            Guard.IsNotNull(encodingToken, nameof(encodingToken));

            CodingTable codingTable = encodingToken.CodingTable;
            inputStream.Reset();
            long sequenceLength = 0;

            while(inputStream.ReadSymbol(out byte symbol)) {
                BitSequence codingSequence = codingTable[symbol];
                foreach(Bit bit in codingSequence) {
                    sequenceLength++;
                    outputStream.WriteBit(bit);
                }
            }
            return sequenceLength;
        }

    }


    [DebuggerDisplay("WeightsTable()")]
    public class WeightsTable : EnumerableBase<WeightedSymbol> {
        readonly long[] data;
        int size;

        public WeightsTable() {
            this.size = 0;
            this.data = new long[256];
        }

        public void TrackSymbol(byte symbol) {
            if(data[symbol] == 0) {
                size++;
            }
            data[symbol]++;
        }
        public void TrackSymbol(byte symbol, long frequency) {
            for(int n = 0; n < frequency; n++)
                TrackSymbol(symbol);
        }
        public int Size {
            get { return size; }
        }
        public long this[byte symbol] {
            get { return data[symbol]; }
        }

        protected override IEnumerator<WeightedSymbol> CreateEnumerator() {
            for(int n = 0; n < data.Length; n++) {
                if(data[n] != 0) yield return new WeightedSymbol((byte)n, data[n]);
            }
        }
    }


    [DebuggerDisplay("WeightedSymbol({Symbol}={Weight})")]
    public struct WeightedSymbol : IEquatable<WeightedSymbol> {
        public WeightedSymbol(byte symbol, long weight) {
            this.Symbol = symbol;
            this.Weight = weight;
        }
        public readonly byte Symbol;
        public readonly long Weight;

        #region Equals & GetHashCode

        public bool Equals(WeightedSymbol other) {
            return Symbol == other.Symbol && Weight == other.Weight;
        }
        public override bool Equals(object obj) {
            return Equals((WeightedSymbol)obj);
        }
        public override int GetHashCode() { return 0; }

        #endregion
    }


    [DebuggerDisplay("CodingTable()")]
    public class CodingTable : EnumerableBase<Pair<byte, BitSequence>> {
        readonly BitSequence[] data;

        public CodingTable() {
            this.data = new BitSequence[256];
        }
        public BitSequence this[byte symbol] {
            get { return data[symbol]; }
            set { data[symbol] = value; }
        }

        protected override IEnumerator<Pair<byte, BitSequence>> CreateEnumerator() {
            for(int n = 0; n < data.Length; n++) {
                if(data[n] != null)
                    yield return new Pair<byte, BitSequence>((byte)n, data[n]);
            }
        }
    }
}