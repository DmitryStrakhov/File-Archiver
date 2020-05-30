using System;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;

namespace FileArchiver.HuffmanCore {
    public class HuffmanDecoder {
        public HuffmanDecoder() {
        }
        public void Decode(IDecodingInputStream inputStream, HuffmanTree tree, IDecodingOutputStream outputStream) {
            Guard.IsNotNull(inputStream, nameof(inputStream));
            Guard.IsNotNull(tree, nameof(tree));
            Guard.IsNotNull(outputStream, nameof(outputStream));

            TreeWalker walker = new TreeWalker(tree, inputStream, outputStream);
            while(!inputStream.IsEmpty) {
                if(!tree.Walk(walker))
                    throw new ArgumentException();
            }
        }

        class TreeWalker : IHuffmanTreeWalker {
            readonly HuffmanTree tree;
            readonly IDecodingInputStream input;
            readonly IDecodingOutputStream output;

            public TreeWalker(HuffmanTree tree, IDecodingInputStream input, IDecodingOutputStream output) {
                this.tree = tree;
                this.input = input;
                this.output = output;
            }
            public TreeWalkStep VisitNode(HuffmanTreeNode node) {
                if(node.IsLeaf) {
                    output.WriteSymbol(node.Value.Symbol);
                    if(tree.IsRoot(node)) {
                        input.ReadBit(out Bit _);
                    }
                    return TreeWalkStep.Finish;
                }
                
                if(!input.ReadBit(out Bit bit))
                    throw new ArgumentException();
                return bit == Bit.Zero ? TreeWalkStep.ToLeft : TreeWalkStep.ToRight;
            }
        }
    }
}