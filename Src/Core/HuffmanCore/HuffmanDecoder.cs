using System;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.HuffmanCore {
    public class HuffmanDecoder {
        public HuffmanDecoder() {
        }

        public void Decode(IDecodingInputStream inputStream, WeightsTable weightsTable, IDecodingOutputStream outputStream, long sequenceLength) {
            Guard.IsNotNull(inputStream, nameof(inputStream));
            Guard.IsNotNull(weightsTable, nameof(weightsTable));
            Guard.IsNotNull(outputStream, nameof(outputStream));
            Guard.IsNotNegative(sequenceLength, nameof(sequenceLength));

            HuffmanTreeBase tree = EncodingTools.BuildHuffmanTree(weightsTable);
            Decode(inputStream, tree, outputStream, sequenceLength);
        }
        public void Decode(IDecodingInputStream inputStream, HuffmanTreeBase tree, IDecodingOutputStream outputStream, long sequenceLength) {
            Guard.IsNotNull(inputStream, nameof(inputStream));
            Guard.IsNotNull(tree, nameof(tree));
            Guard.IsNotNull(outputStream, nameof(outputStream));
            Guard.IsNotNegative(sequenceLength, nameof(sequenceLength));

            TreeWalker walker = new TreeWalker(tree, inputStream, outputStream, sequenceLength);
            while(!walker.Exhausted && !inputStream.IsEmpty) {
                if(!tree.Walk(walker))
                    throw new ArgumentException();
            }
        }

        class TreeWalker : IHuffmanTreeWalker {
            readonly HuffmanTreeBase tree;
            readonly IDecodingInputStream input;
            readonly IDecodingOutputStream output;
            long seqLength;

            public TreeWalker(HuffmanTreeBase tree, IDecodingInputStream input, IDecodingOutputStream output, long seqLength) {
                this.tree = tree;
                this.input = input;
                this.output = output;
                this.seqLength = seqLength;
            }
            public TreeWalkStep VisitNode(HuffmanTreeNode node) {
                if(node.IsLeaf) {
                    output.WriteSymbol(node.Value.Symbol);
                    if(tree.IsRoot(node)) {
                        if(input.ReadBit(out Bit _))
                            seqLength--;
                    }
                    return TreeWalkStep.Finish;
                }
                
                if(!input.ReadBit(out Bit bit))
                    throw new ArgumentException();
                seqLength--;
                return bit == Bit.Zero ? TreeWalkStep.ToLeft : TreeWalkStep.ToRight;
            }
            public bool Exhausted => seqLength == 0;
        }
    }
}