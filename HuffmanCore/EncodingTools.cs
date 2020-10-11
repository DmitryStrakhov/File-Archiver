using FileArchiver.DataStructures;
using FileArchiver.Helpers;

namespace FileArchiver.HuffmanCore {
    public static class EncodingTools {
        public static WeightsTable BuildWeightsTable(IEncodingInputStream inputStream) {
            Guard.IsNotNull(inputStream, nameof(inputStream));
            WeightsTable weightsTable = new WeightsTable();

            while(inputStream.ReadSymbol(out byte symbol)) {
                weightsTable.TrackSymbol(symbol);
            }
            return weightsTable;
        }
        public static  HuffmanTreeBase BuildHuffmanTree(WeightsTable weightsTable) {
            Guard.IsNotNull(weightsTable, nameof(weightsTable));
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
        public static CodingTable BuildCodingTable(HuffmanTreeBase huffmanTree) {
            Guard.IsNotNull(huffmanTree, nameof(huffmanTree));
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
}