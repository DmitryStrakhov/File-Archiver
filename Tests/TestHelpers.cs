#if DEBUG

using System;
using System.Diagnostics;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Tests {
    public static class HuffmanTreeHelper {
        public static TreeNodeBuildContext Builder() {
            return new TreeNodeBuildContext();
        }
        public static HuffmanTree SomeTree() {
            return Builder().NewNode(1, 1).CreateTree();
        }
    }

    public class TreeNodeBuildContext {
        public TreeNodeTraits NewNode(int weight, char? symbol) {
            return TreeNodeTraits.New(weight, (byte?)symbol);
        }
        public TreeNodeTraits NewNode(int weight, byte? symbol) {
            return TreeNodeTraits.New(weight, symbol);
        }
        public TreeNodeTraits NewInternalNode(int weight) {
            return NewNode(weight, 0);
        }
    }

    
    [DebuggerDisplay("TreeNodeTraits('{(char)Symbol.Symbol}', Weight={Symbol.Weight})")]
    public class TreeNodeTraits {
        private TreeNodeTraits(WeightedSymbol symbol) {
            Symbol = symbol;
        }

        public TreeNodeTraits WithLeft(int weight, byte symbol) {
            Left = New(weight, symbol);
            Left.Parent = this;
            return this;
        }
        public TreeNodeTraits WithLeft(int weight, char symbol) {
            return WithLeft(weight, (byte)symbol);
        }
        public TreeNodeTraits WithLeftInternal(int weight) {
            return WithLeft(weight, 0);
        }

        public TreeNodeTraits WithRight(int weight, byte symbol) {
            Right = New(weight, symbol);
            Right.Parent = this;
            return this;
        }
        public TreeNodeTraits WithRight(int weight, char symbol) {
            return WithRight(weight, (byte)symbol);
        }
        public TreeNodeTraits WithRightInternal(int weight) {
            return WithRight(weight, 0);
        }

        public TreeNodeTraits GoToLeft() {
            return Left;
        }
        public TreeNodeTraits GoToRight() {
            return Right;
        }
        public TreeNodeTraits GoToSibling() {
            if(Parent == null) throw new InvalidOperationException();

            if(ReferenceEquals(Parent.Left, this)) {
                return Parent.Right;
            }
            if(ReferenceEquals(Parent.Right, this)) {
                return Parent.Left;
            }
            throw new InvalidOperationException();
        }
        public HuffmanTree CreateTree() {
            return HuffmanTreeBuilder.Build(this);
        }
        
        public static TreeNodeTraits New(int weight, byte? symbol) {
            return new TreeNodeTraits(new WeightedSymbol(symbol.GetValueOrDefault(0), weight));
        }

        public WeightedSymbol Symbol { get; }
        public TreeNodeTraits Parent { get; set; }
        public TreeNodeTraits Left { get; set; }
        public TreeNodeTraits Right { get; set; }
    }


    public static class HuffmanTreeBuilder {
        public static HuffmanTree Build(TreeNodeTraits nodeTraits) {
            Guard.IsNotNull(nodeTraits, nameof(nodeTraits));
            TreeNodeTraits rootTraits = GetRoot(nodeTraits);
            return new HuffmanTree(DoBuild(rootTraits));
        }
        static HuffmanTreeNode DoBuild(TreeNodeTraits nodeTraits) {
            if(nodeTraits == null) return null;

            HuffmanTreeNode l = DoBuild(nodeTraits.Left);
            HuffmanTreeNode r = DoBuild(nodeTraits.Right);
            return new HuffmanTreeNode(nodeTraits.Symbol, l, r);
        }
        static TreeNodeTraits GetRoot(TreeNodeTraits nodeTraits) {
            TreeNodeTraits root = nodeTraits;

            while(root.Parent != null) {
                root = root.Parent;
            }
            return root;
        }
    }
}
#endif