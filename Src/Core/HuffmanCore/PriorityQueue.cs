using System;
using FileArchiver.Core.DataStructures;

namespace FileArchiver.Core.HuffmanCore {
    public class HuffmanTreeNodePriorityQueue : MinimumPriorityQueue<long, HuffmanTreeNode> {
        public HuffmanTreeNodePriorityQueue() : base(256) {
        }
    }
}