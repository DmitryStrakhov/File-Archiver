using System;
using System.Collections.Generic;
using System.Linq;
using FileArchiver.Core;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    #region Streams

    class TestIEncodingInputStream : IEncodingInputStream {
        readonly byte[] data;
        int index;

        public TestIEncodingInputStream(byte[] data) {
            Guard.IsNotNull(data, nameof(data));
            this.data = data;
            this.index = 0;
        }

        public bool ReadSymbol(out byte symbol) {
            bool hasSymbol = index < data.Length;
            symbol = hasSymbol ? data[index++] : (byte)0;
            return hasSymbol;
        }
        public string Path {
            get { return "[test]"; }
        }
        public void Reset() {
            index = 0;
        }
        public void Dispose() {
        }
    }

    class TestIEncodingOutputStream : IEncodingOutputStream {
        readonly List<Bit> bitList;
        int index;

        public TestIEncodingOutputStream() {
            this.index = 0;
            this.bitList = new List<Bit>(64);
        }

        #region IEncodingOutputStream

        void IEncodingOutputStream.BeginWrite() {
        }
        void IEncodingOutputStream.EndWrite() {
        }
        void IEncodingOutputStream.WriteBit(Bit bit) {
            if(index == bitList.Count)
                bitList.Add(bit);
            else
                bitList[index] = bit;
            index++;
        }
        IStreamPosition IEncodingOutputStream.SavePosition() {
            return new StreamPosition(index);
        }
        void IEncodingOutputStream.RestorePosition(IStreamPosition position) {
            index = ((StreamPosition)position).Index;
        }
        void IDisposable.Dispose() {
        }

        #endregion

        #region StreamPosition

        class StreamPosition : IStreamPosition {
            public StreamPosition(int index) {
                Index = index;
            }
            public readonly int Index;
        }

        #endregion

        public IReadOnlyList<Bit> BitList {
            get { return bitList; }
        }
    }

    #endregion

    [TestFixture]
    public class HuffmanEncoderTests {
        [Test]
        public void CreateEncodingTokenGuardTest() {
            Assert.Throws<ArgumentNullException>(() => new HuffmanEncoder().CreateEncodingToken(null));
        }
        [Test]
        public void EncodeGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(null, new TestIEncodingOutputStream(), EmptyEncodingToken.Instance, null)
            );
        }
        [Test]
        public void EncodeGuardCase2Test() {
            Assert.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(new TestIEncodingInputStream(new byte[0]), null, EmptyEncodingToken.Instance, null)
            );
        }
        [Test]
        public void EncodeGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(new TestIEncodingInputStream(new byte[0]), new TestIEncodingOutputStream(), null, null)
            );
        }

        [Test]
        public void BuildWeightsTableTest1() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[0]);
            WeightsTable weightsTable = encoder.BuildWeightsTable(inputStream);
            Assert.IsNotNull(weightsTable);
            CollectionAssert.IsEmpty(weightsTable);
        }
        [Test]
        public void BuildWeightsTableTest2() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[] {1});
            WeightsTable weightsTable = encoder.BuildWeightsTable(inputStream);
            Assert.IsNotNull(weightsTable);
            WeightedSymbol[] expected = {new WeightedSymbol(1, 1)};
            CollectionAssert.AreEqual(expected, weightsTable.ToArray());
        }
        [Test]
        public void BuildWeightsTableTest3() {
            byte[] data = @"aaaabbbccedddddeeeaabdcefffeffadc".ToByteArray();
            HuffmanEncoder encoder = new HuffmanEncoder();

            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            WeightsTable weightsTable = encoder.BuildWeightsTable(inputStream);
            Assert.IsNotNull(weightsTable);
            WeightedSymbol[] expected = {
                new WeightedSymbol((byte)'a', 7),
                new WeightedSymbol((byte)'b', 4),
                new WeightedSymbol((byte)'c', 4),
                new WeightedSymbol((byte)'d', 7),
                new WeightedSymbol((byte)'e', 6),
                new WeightedSymbol((byte)'f', 5),
            };
            CollectionAssert.AreEqual(expected, weightsTable.ToArray());
        }

        [Test]
        public void BuildHuffmanTreeTest1() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            WeightsTable weightsTable = new WeightsTable();
            HuffmanTreeBase huffmanTree = encoder.BuildHuffmanTree(weightsTable);
            Assert.IsNotNull(huffmanTree);
            CollectionAssert.IsEmpty(huffmanTree.FlattenValues());
        }
        [Test]
        public void BuildHuffmanTreeTest2() {
            WeightsTable weightsTable = new WeightsTable();
            weightsTable.TrackSymbol(1);
            HuffmanEncoder encoder = new HuffmanEncoder();

            HuffmanTreeBase huffmanTree = encoder.BuildHuffmanTree(weightsTable);
            Assert.IsNotNull(huffmanTree);
            WeightedSymbol?[] expected = {
                new WeightedSymbol(1, 1), null, null
            };
            Assert.AreEqual(expected, huffmanTree.FlattenValues());
        }
        [Test]
        public void BuildHuffmanTreeTest3() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            WeightsTable weightsTable = new WeightsTable();
            for(int n = 0; n < 12; n++) {
                weightsTable.TrackSymbol((byte)'a');
            }
            for(int n = 0; n < 2; n++) {
                weightsTable.TrackSymbol((byte)'b');
            }
            for(int n = 0; n < 7; n++) {
                weightsTable.TrackSymbol((byte)'c');
            }
            for(int n = 0; n < 13; n++) {
                weightsTable.TrackSymbol((byte)'d');
            }
            for(int n = 0; n < 14; n++) {
                weightsTable.TrackSymbol((byte)'e');
            }
            for(int n = 0; n < 85; n++) {
                weightsTable.TrackSymbol((byte)'f');
            }

            HuffmanTreeBase huffmanTree = encoder.BuildHuffmanTree(weightsTable);
            Assert.IsNotNull(huffmanTree);
            WeightedSymbol?[] expected = {
                new WeightedSymbol(0, 133),
                new WeightedSymbol(0, 048),
                new WeightedSymbol((byte)'f', 085),
                new WeightedSymbol(0, 021),
                new WeightedSymbol(0, 027),
                null,
                null,
                new WeightedSymbol(0, 009),
                new WeightedSymbol((byte)'a', 012),
                new WeightedSymbol((byte)'d', 013),
                new WeightedSymbol((byte)'e', 014),
                new WeightedSymbol((byte)'b', 002),
                new WeightedSymbol((byte)'c', 007),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
            };
            Assert.AreEqual(expected, huffmanTree.FlattenValues());
        }
        [Test]
        public void BuildHuffmanTreeTest4() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            WeightsTable weightsTable = new WeightsTable();
            for(int n = 0; n < 13; n++) {
                weightsTable.TrackSymbol((byte)'A');
            }
            weightsTable.TrackSymbol((byte)'B');

            for(int n = 0; n < 2; n++) {
                weightsTable.TrackSymbol((byte)'C');
            }
            for(int n = 0; n < 15; n++) {
                weightsTable.TrackSymbol((byte)'D');
            }
            for(int n = 0; n < 11; n++) {
                weightsTable.TrackSymbol((byte)'E');
            }

            HuffmanTreeBase huffmanTree = encoder.BuildHuffmanTree(weightsTable);
            Assert.IsNotNull(huffmanTree);
            WeightedSymbol?[] expected = {
                new WeightedSymbol(0, 42),
                new WeightedSymbol((byte)'D', 15),
                new WeightedSymbol(0, 27),
                null,
                null,
                new WeightedSymbol((byte)'A', 13),
                new WeightedSymbol(0, 14),
                null,
                null,
                new WeightedSymbol(0, 03),
                new WeightedSymbol((byte)'E', 11),
                new WeightedSymbol((byte)'B', 01),
                new WeightedSymbol((byte)'C', 02),
                null,
                null,
                null,
                null,
                null,
                null,
            };
            Assert.AreEqual(expected, huffmanTree.FlattenValues());
        }

        [Test]
        public void BuildCodingTableTest1() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            CodingTable codingTable = encoder.BuildCodingTable(EmptyHuffmanTree.Instance);
            CollectionAssert.IsEmpty(codingTable);
        }
        [Test]
        public void BuildCodingTableTest2() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewNode(1, 'X').CreateTree();
            CodingTable codingTable = encoder.BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);

            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'X', BitSequence.FromString("0"))
            };
            CollectionAssert.AreEquivalent(expected, codingTable);
        }
        [Test]
        public void BuildCodingTableTest3() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(15)
                .WithLeft(5, 'A')
                .WithRight(10, 'B')
                .CreateTree();

            CodingTable codingTable = encoder.BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);

            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'A', BitSequence.FromString("0")),
                new Pair<byte, BitSequence>((byte)'B', BitSequence.FromString("1")),
            };
            CollectionAssert.AreEquivalent(expected, codingTable);
        }
        [Test]
        public void BuildCodingTableTest4() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(133)
                .WithRight(85, 'f')
                .WithLeftInternal(48)
                .GoToLeft()
                .WithLeftInternal(21)
                .WithRightInternal(27)
                .GoToRight()
                .WithLeft(13, 'd')
                .WithRight(14, 'e')
                .GoToSibling()
                .WithRight(12, 'a')
                .WithLeftInternal(9)
                .GoToLeft()
                .WithRight(7, 'c')
                .WithLeft(2, 'b')
                .CreateTree();

            CodingTable codingTable = encoder.BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);

            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'a', BitSequence.FromString("001")),
                new Pair<byte, BitSequence>((byte)'b', BitSequence.FromString("0000")),
                new Pair<byte, BitSequence>((byte)'c', BitSequence.FromString("0001")),
                new Pair<byte, BitSequence>((byte)'d', BitSequence.FromString("010")),
                new Pair<byte, BitSequence>((byte)'e', BitSequence.FromString("011")),
                new Pair<byte, BitSequence>((byte)'f', BitSequence.FromString("1")),
            };
            CollectionAssert.AreEquivalent(expected, codingTable);
        }
        [Test]
        public void BuildCodingTableTest5() {
            HuffmanEncoder encoder = new HuffmanEncoder();
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(44)
                .WithLeft(19, 'C')
                .WithRightInternal(25)
                .GoToRight()
                .WithLeft(11, 'B')
                .WithRightInternal(14)
                .GoToRight()
                .WithRight(9, 'E')
                .WithLeftInternal(5)
                .GoToLeft()
                .WithLeft(2, 'D')
                .WithRight(3, 'A')
                .CreateTree();

            CodingTable codingTable = encoder.BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);

            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'A', BitSequence.FromString("1101")),
                new Pair<byte, BitSequence>((byte)'B', BitSequence.FromString("10")),
                new Pair<byte, BitSequence>((byte)'C', BitSequence.FromString("0")),
                new Pair<byte, BitSequence>((byte)'D', BitSequence.FromString("1100")),
                new Pair<byte, BitSequence>((byte)'E', BitSequence.FromString("111")),
            };
            CollectionAssert.AreEquivalent(expected, codingTable);
        }

        
        [Test]
        public void EncodeTest1() {
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[0]);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();

            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            long length = encoder.Encode(inputStream, outputStream, token, null);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);
            Assert.AreEqual(0, length);
            CollectionAssert.IsEmpty(token.HuffmanTree.FlattenValues());
            CollectionAssert.IsEmpty(outputStream.BitList);
        }
        [Test]
        public void EncodeTest2() {
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[] { 1 });
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            long length = encoder.Encode(inputStream, outputStream, token, null);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);

            WeightedSymbol?[] expected = {
                new WeightedSymbol(1, 1), 
                null,
                null,
            };
            Assert.AreEqual(expected, token.HuffmanTree.FlattenValues());
            Assert.AreEqual(1, length);
            AssertOutputStream(outputStream, "0");
        }
        [Test]
        public void EncodeTest3() {
            byte[] data = { 1, 2, 2, 1, 1, 2, 2 };
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            long length = encoder.Encode(inputStream, outputStream, token, null);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);

            WeightedSymbol?[] expected = {
                new WeightedSymbol(0, 7), 
                new WeightedSymbol(1, 3), 
                new WeightedSymbol(2, 4), 
                null,
                null,
                null,
                null,
            };
            Assert.AreEqual(expected, token.HuffmanTree.FlattenValues());
            Assert.AreEqual(7, length);
            AssertOutputStream(outputStream, "0110011");
        }
        [Test]
        public void EncodeTest4() {
            byte[] data = { 1, 2, 2, 2, 2, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4 };
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            long length = encoder.Encode(inputStream, outputStream, token, null);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);
            Assert.AreEqual(54, length);
            AssertOutputStream(outputStream, "110111111111111110111111111101010101010001010000000000");
        }
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void EncodingProgressTest1(int size) {
            byte[] data = new byte[size];
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            TestIProgressHandler progress = new TestIProgressHandler();
            encoder.Encode(inputStream, outputStream, token, progress);
            CollectionAssert.IsEmpty(progress.ValueList);
        }
        [Test]
        public void EncodingProgressTest2() {
            byte[] data = new byte[27 * 1024];
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            TestIProgressHandler progress = new TestIProgressHandler();

            for(int n = 0; n < 10; n++) {
                encoder.Encode(inputStream, outputStream, token, progress);
                inputStream.Reset();
            }
            CollectionAssert.AreEqual(new long[]{ 128 * 1024, 128 * 1024 }, progress.ValueList);
        }

        private void AssertOutputStream(TestIEncodingOutputStream stream, string expected) {
            IReadOnlyList<Bit> bitList = stream.BitList;
            if(bitList.Count != expected.Length) throw new AssertionException(nameof(stream));

            for(int n = 0; n < bitList.Count; n++) {
                if(bitList[n] != CharToBit(expected[n]))
                    throw new AssertionException(nameof(stream));
            }
        }
        private static Bit CharToBit(char @char) {
            switch(@char) {
                case '0': return Bit.Zero;
                case '1': return Bit.One;
                default: throw new ArgumentException();    
            }
        }
    }
}
