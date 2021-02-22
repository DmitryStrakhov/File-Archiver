using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Tests {
    public static class HuffmanTreeTestExtensions {
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

    public static class ConversionTool {
        public static char ToHexSymbol(byte value) {
            if(value > 15) {
                throw new ArgumentException();
            }
            return value <= 9 ? (char)('0' + value) : (char)('A' + value - 10);
        }
    }

    public static class ByteListTestExtensions {
        public static string ToHexString(this IReadOnlyList<byte> @this) {
            char[] chars = new char[@this.Count * 2 + 2];

            int index = chars.Length - 1;
            foreach(byte b in @this) {
                var pair = ToHexSymbol(b);
                chars[index--] = pair.Item1;
                chars[index--] = pair.Item2;
            }
            chars[0] = '0';
            chars[1] = 'x';
            return new string(chars);
        }
        private static Tuple<char, char> ToHexSymbol(byte value) {
            char digit1 = ConversionTool.ToHexSymbol((byte)(value & 0xF));
            char digit2 = ConversionTool.ToHexSymbol((byte)((value >> 4) & 0xF));
            return new Tuple<char, char>(digit1, digit2);
        }
    }

    public static class BitListTestExtensions {
        public static string ToHexString(this IReadOnlyList<Bit> @this) {
            char[] chars = new char[@this.Count / 4 + 2];

            ByteWriter bw = new ByteWriter();
            for(int n = 0; n < @this.Count;) {
                for(int j = 0; j < 4; j++) {
                    bw.AddBit(@this[n++]);
                }
                chars[chars.Length - n / 4] = ConversionTool.ToHexSymbol(bw.Value);
                bw.Reset();
            }
            chars[0] = '0';
            chars[1] = 'x';
            return new string(chars);
        }
    }

    public static class ContentControlExtensions {
        public static T GetContentTemplateControl<T>(this ContentControl @this, string controlName)
            where T : UIElement {

            if(VisualTreeHelper.GetChildrenCount(@this) == 0) {
                throw new ArgumentException(nameof(@this));
            }

            ContentPresenter contentPresenter = (ContentPresenter)VisualTreeHelper.GetChild(@this, 0);
            if(contentPresenter == null) {
                throw new ArgumentException(nameof(@this));
            }
            return (T)@this.ContentTemplate.FindName(controlName, contentPresenter);
        }
    }
}
