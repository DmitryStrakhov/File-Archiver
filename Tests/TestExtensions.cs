#if DEBUG

using System;
using FileArchiver.HuffmanCore;
using System.Collections.Generic;
using FileArchiver.Helpers;

namespace FileArchiver.Tests {
    public static class TestExtensions {
        public static IReadOnlyCollection<WeightedSymbol?> FlattenValues(this HuffmanTreeBase @this) {
            Guard.IsNotNull(@this, nameof(@this));
            TreeVisitor visitor = new TreeVisitor();
            @this.DoBFS(visitor);
            return visitor.ValueList;
        }

        class TreeVisitor : IHuffmanTreeVisitor {
            readonly List<WeightedSymbol?> valueList = new List<WeightedSymbol?>(64);

            public void VisitNode(HuffmanTreeNode node) {
                valueList.Add(node?.Value);
            }
            public void VisitLeftEdge(int level) {
            }
            public void VisitBackEdge(int level) {
            }
            public void VisitRightEdge(int level) {
            }
            public IReadOnlyCollection<WeightedSymbol?> ValueList { get { return valueList; } }
        }
    }
}
#endif