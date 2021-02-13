using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FileArchiver.Core.Base;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Tests {
    [DebuggerDisplay("BufferBuilder()")]
    public class BufferBuilder {
        readonly MemoryStream memoryStream;

        public BufferBuilder() {
            this.memoryStream = new MemoryStream(128);
        }

        public BufferBuilder AddByte(byte value) {
            memoryStream.WriteByte(value);
            return this;
        }
        public BufferBuilder AddByte(byte value, int times) {
            for(int n = 0; n < times; n++) AddByte(value);
            return this;
        }
        public BufferBuilder AddInt(int value) {
            for(int n = 0; n < 4; n++) {
                AddByte((byte)value);
                value >>= 8;
            }
            return this;
        }
        public BufferBuilder AddLong(long value) {
            for(int n = 0; n < 8; n++) {
                AddByte((byte)value);
                value >>= 8;
            }
            return this;
        }
        public BufferBuilder AddString(string value) {
            for(int n = 0; n < value.Length; n++) {
                AddByte((byte)value[n]);
                AddByte((byte)(value[n] >> 8));
            }
            return this;
        }
        public byte[] GetData() {
            return memoryStream.ToArray();
        }
        public Stream GetStream() {
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }

    public static class BitListHelper {
        public static BitListBuilder CreateBuilder() {
            return new BitListBuilder();
        }
    }

    public class BitListBuilder {
        readonly List<Bit> bitList;

        public BitListBuilder() {
            this.bitList = new List<Bit>(64);
        }
        public BitListBuilder AddByte(byte value) {
            for(int n = 0; n < 8; n++) {
                bitList.Add((value & 1) == 1 ? Bit.One : Bit.Zero);
                value >>= 1;
            }
            return this;
        }
        public BitListBuilder AddString(string value) {
            for(int n = 0; n < value.Length; n++) {
                AddChar(value[n]);
            }
            return this;
        }
        public BitListBuilder AddChar(char symbol) {
            for(int n = 0; n < 2; n++) {
                AddByte((byte)symbol);
                symbol >>= 8;
            }
            return this;
        }
        public BitListBuilder AddInt(int value) {
            for(int n = 0; n < 4; n++) {
                AddByte((byte)value);
                value >>= 8;
            }
            return this;
        }
        public BitListBuilder AddLong(long value) {
            for(int n = 0; n < 8; n++) {
                AddByte((byte)value);
                value >>= 8;
            }
            return this;
        }
        public IReadOnlyList<Bit> BitList { get { return bitList; } }
    }


    public static class FileSystemEntryHelper {
        public static FileSystemEntryListBuilder CreateListBuilder() {
            return new FileSystemEntryListBuilder();
        }
    }

    public class FileSystemEntryListBuilder {
        readonly List<FileSystemEntry> entryList;
        string currentDir;

        public FileSystemEntryListBuilder() {
            this.currentDir = null;
            this.entryList = new List<FileSystemEntry>(64);
        }
        public FileSystemEntryListBuilder AddDirectory(string path, int cardinality) {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            currentDir = path;
            entryList.Add(NewDirectory(path, cardinality));
            return this;
        }
        public FileSystemEntryListBuilder AddFile(string name) {
            Guard.IsNotNullOrEmpty(name, nameof(name));
            if(string.IsNullOrEmpty(currentDir)) {
                throw new InvalidOperationException();
            }
            entryList.Add(NewFile(name));
            return this;
        }
        public FileSystemEntryListBuilder AddFiles(params string[] names) {
            Guard.IsNotNull(names, nameof(names));
            for(int n = 0; n < names.Length; n++) {
                AddFile(names[n]);
            }
            return this;
        }
        public IReadOnlyList<FileSystemEntry> GetList() => entryList;

        private FileSystemEntry NewDirectory(string path, int cardinality) {
            string name = PathHelper.GetDirectoryName(path);
            return new FileSystemEntry(FileSystemEntryType.Directory, name, path, cardinality);
        }
        private FileSystemEntry NewFile(string name) {
            string path = Path.Combine(currentDir, name);
            return new FileSystemEntry(FileSystemEntryType.File, name, path);
        }
    }

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

    public static class TestHelper {
        public static Bit[] BitsFromString(string @string) {
            Bit[] result = new Bit[@string.Length];

            for(int n = 0; n < @string.Length; n++) {
                switch(@string[n]) {
                    case '0':
                        result[n] = Bit.Zero;
                        break;
                    case '1':
                        result[n] = Bit.One;
                        break;
                    default: throw new ArgumentException();
                }
            }
            return result;
        }
        public static string StringFromBits(Bit[] bits) {
            char[] chars = new char[bits.Length];

            for(int n = 0; n < bits.Length; n++) {
                chars[n] = bits[n] == Bit.Zero ? '0' : '1';
            }
            return new string(chars);
        }
        public static string CreateString(string pattern, int repeat) {
            StringBuilder stringBuilder = new StringBuilder(pattern.Length * repeat);

            for(int n = 0; n < repeat; n++) {
                stringBuilder.Append(pattern);
            }
            return stringBuilder.ToString();
        }
    }
}
