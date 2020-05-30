using System;
using System.Collections.Generic;
using System.Diagnostics;
using FileArchiver.Helpers;

namespace FileArchiver.HuffmanCore {
    public abstract class HuffmanTreeBase {
        public abstract bool IsRoot(HuffmanTreeNode node);
        public abstract void DoBFS(IHuffmanTreeVisitor visitor);
        public abstract void DoPOT(IHuffmanTreeVisitor visitor);
        public abstract bool Walk(IHuffmanTreeWalker walker);
    }

    
    public interface IHuffmanTreeVisitor {
        void VisitNode(HuffmanTreeNode node);
        void VisitLeftEdge(int level);
        void VisitBackEdge(int level);
        void VisitRightEdge(int level);
    }

    public interface IHuffmanTreeWalker {
        TreeWalkStep VisitNode(HuffmanTreeNode node);
    }

    public enum TreeWalkStep {
        ToLeft, ToRight, Finish
    }

    [DebuggerDisplay("HuffmanTree()")]
    public class HuffmanTree : HuffmanTreeBase {
        readonly HuffmanTreeNode root;

        public HuffmanTree(HuffmanTreeNode root) {
            Guard.IsNotNull(root, nameof(root));
            this.root = root;
        }

        public override bool IsRoot(HuffmanTreeNode node) {
            return ReferenceEquals(node, root);
        }
        public override void DoBFS(IHuffmanTreeVisitor visitor) {
            Guard.IsNotNull(visitor, nameof(visitor));
            
            Queue<HuffmanTreeNode> bfsQueue = new Queue<HuffmanTreeNode>(64);
            bfsQueue.Enqueue(root);
            while(bfsQueue.Count != 0) {
                HuffmanTreeNode node = bfsQueue.Dequeue();
                visitor.VisitNode(node);
                if(node == null) continue;
                bfsQueue.Enqueue(node.Left);
                bfsQueue.Enqueue(node.Right);
            }
        }
        public override void DoPOT(IHuffmanTreeVisitor visitor) {
            HuffmanTreeNode node = root;
            int level = 0;

            Stack<HuffmanTreeNode> stack = new Stack<HuffmanTreeNode>();
            while(true) {
                if(node != null) {
                    stack.Push(node);
                    node = node.Left;
                    visitor.VisitLeftEdge(++level);
                }
                else {
                    if(stack.Count == 0) return;

                    if(stack.Peek().Right == null) {
                        visitor.VisitBackEdge(--level);
                        node = stack.Pop();
                        if(node.IsLeaf) {
                            visitor.VisitNode(node);
                        }

                        while(stack.Count != 0 && node == stack.Peek().Right) {
                            visitor.VisitBackEdge(--level);
                            node = stack.Pop();
                            if(node.IsLeaf) {
                                visitor.VisitNode(node);
                            }
                        }
                    }
                    if(stack.Count != 0) {
                        node = stack.Peek().Right;
                        visitor.VisitRightEdge(level);
                    }
                    else node = null;
                }
            }
        }
        public override bool Walk(IHuffmanTreeWalker walker) {
            HuffmanTreeNode node = root;

            while(node != null) {
                TreeWalkStep walkStep = walker.VisitNode(node);
                switch(walkStep) {
                    case TreeWalkStep.ToLeft:
                        node = node.Left;
                        break;
                    case TreeWalkStep.ToRight:
                        node = node.Right;
                        break;
                    case TreeWalkStep.Finish: return true;
                    default: throw new InvalidOperationException();
                }
            }
            return false;
        }
    }


    [DebuggerDisplay("EmptyHuffmanTree()")]
    public class EmptyHuffmanTree : HuffmanTreeBase {
        private EmptyHuffmanTree() {
        }
        public override bool IsRoot(HuffmanTreeNode node) { return false; }
        public override void DoBFS(IHuffmanTreeVisitor visitor) {
        }
        public override void DoPOT(IHuffmanTreeVisitor visitor) {
        }
        public override bool Walk(IHuffmanTreeWalker walker) {
            return true;
        }
        public static readonly EmptyHuffmanTree Instance = new EmptyHuffmanTree();
    }


    [DebuggerDisplay("HuffmanTreeNode('{(char)Value.Symbol}',{Value.Weight})")]
    public class HuffmanTreeNode {
        public HuffmanTreeNode(WeightedSymbol value) : this(value, null, null) {
        }
        public HuffmanTreeNode(WeightedSymbol value, HuffmanTreeNode left, HuffmanTreeNode right) {
            Value = value;
            Left = left;
            Right = right;
        }
        public readonly WeightedSymbol Value;
        public readonly HuffmanTreeNode Left;
        public readonly HuffmanTreeNode Right;
        public bool IsLeaf { get { return Left == null && Right == null; } }
    }
}