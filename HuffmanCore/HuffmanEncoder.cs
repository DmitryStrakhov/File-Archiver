using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FileArchiver.Helpers;
using FileArchiver.DataStructures;

namespace FileArchiver.HuffmanCore {
    public class HuffmanEncoder {
        public HuffmanEncoder() {
        }
        public void Encode(IEncodingInputStream inputStream, IEncodingOutputStream outputStream, out HuffmanTreeBase huffmanTree) {
            Guard.IsNotNull(inputStream, nameof(inputStream));
            Guard.IsNotNull(outputStream, nameof(outputStream));

            WeightsTable weightsTable = BuildWeightsTable(inputStream);
            huffmanTree = BuildHuffmanTree(weightsTable);
            CodingTable codingTable = BuildCodingTable(huffmanTree);
            
            inputStream.Reset();
            while(inputStream.ReadSymbol(out byte symbol)) {
                BitSequence codingSequence = codingTable[symbol];
                foreach(Bit bit in codingSequence) {
                    outputStream.WriteBit(bit);
                }
            }
        }
        internal WeightsTable BuildWeightsTable(IEncodingInputStream inputStream) {
            WeightsTable weightsTable = new WeightsTable();

            while(inputStream.ReadSymbol(out byte symbol)) {
                weightsTable.TrackSymbol(symbol);
            }
            return weightsTable;
        }
        internal HuffmanTreeBase BuildHuffmanTree(WeightsTable weightsTable) {
            HuffmanTreeNodePriorityQueue priorityQueue = new HuffmanTreeNodePriorityQueue();

            foreach(WeightedSymbol weightedSymbol in weightsTable) {
                priorityQueue.Insert(weightedSymbol.Weight, new HuffmanTreeNode(weightedSymbol));
            }
            if(priorityQueue.Size == 0) return EmptyHuffmanTree.Instance;

            while(priorityQueue.Size > 1) {
                var lNode = priorityQueue.DeleteMinimumValue();
                var rNode = priorityQueue.DeleteMinimumValue();
                long weight = lNode.Value.Weight + rNode.Value.Weight;
                var internalNode = new HuffmanTreeNode(new WeightedSymbol(0, weight), lNode, rNode);
                priorityQueue.Insert(weight, internalNode);
            }
            return new HuffmanTree(priorityQueue.GetMinimumValue());
        }
        internal CodingTable BuildCodingTable(HuffmanTreeBase huffmanTree) {
            TreeVisitor visitor = new TreeVisitor();
            huffmanTree.DoPOT(visitor);
            return visitor.CodingTable;
        }

        class TreeVisitor : IHuffmanTreeVisitor {
            readonly BitSequence bitSequence;
            readonly CodingTable codingTable;

            public TreeVisitor() {
                this.codingTable = new CodingTable();
                this.bitSequence = new BitSequence();
            }

            public void VisitNode(HuffmanTreeNode node) {
                codingTable[node.Value.Symbol] = bitSequence.Clone();
            }
            public void VisitLeftEdge(int level) {
                bitSequence[level - 1] = Bit.Zero;
            }
            public void VisitBackEdge(int level) {
                if(level > 0)
                    bitSequence.Reduce(level - 1);
            }
            public void VisitRightEdge(int level) {
                bitSequence[level - 1] = Bit.One;
            }
            public CodingTable CodingTable { get { return codingTable; } }
        }
    }


    [DebuggerDisplay("WeightsTable()")]
    public class WeightsTable : EnumerableBase<WeightedSymbol> {
        readonly long[] data;

        public WeightsTable() {
            this.data = new long[256];
        }

        public void TrackSymbol(byte symbol) {
            data[symbol]++;
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