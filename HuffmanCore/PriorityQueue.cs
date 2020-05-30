using System;
using FileArchiver.DataStructures;

namespace FileArchiver.HuffmanCore {
    public class HuffmanTreeNodePriorityQueue : MinimumPriorityQueue<long, HuffmanTreeNode> {
        public HuffmanTreeNodePriorityQueue() : base(256) {
        }
    }
}