#if DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        #region IEncodingInputStream

        bool IEncodingInputStream.ReadSymbol(out byte symbol) {
            bool hasSymbol = index < data.Length;
            symbol = hasSymbol ? data[index++] : (byte)0;
            return hasSymbol;
        }
        void IEncodingInputStream.Reset() {
            index = 0;
        }
        void IDisposable.Dispose() {
        }

        #endregion
    }

    class TestIEncodingOutputStream : IEncodingOutputStream {
        readonly List<Bit> bitList;

        public TestIEncodingOutputStream() {
            this.bitList = new List<Bit>(64);
        }

        #region IEncodingOutputStream

        void IEncodingOutputStream.BeginWrite() {
        }
        void IEncodingOutputStream.EndWrite() {
        }
        void IEncodingOutputStream.WriteBit(Bit bit) {
            bitList.Add(bit);
        }
        void IDisposable.Dispose() {
        }

        #endregion

        public IReadOnlyList<Bit> BitList {
            get { return bitList; }
        }
    }

    #endregion

    [TestClass]
    public class HuffmanEncoderTests {
        [TestMethod]
        public void CreateEncodingTokenGuardTest() {
            AssertHelper.Throws<ArgumentNullException>(() => new HuffmanEncoder().CreateEncodingToken(null));
        }
        [TestMethod]
        public void EncodeGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(null, new TestIEncodingOutputStream(), EmptyEncodingToken.Instance)
            );
        }
        [TestMethod]
        public void EncodeGuardCase2Test() {
            AssertHelper.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(new TestIEncodingInputStream(new byte[0]), null, EmptyEncodingToken.Instance)
            );
        }
        [TestMethod]
        public void EncodeGuardCase3Test() {
            AssertHelper.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(new TestIEncodingInputStream(new byte[0]), new TestIEncodingOutputStream(), null)
            );
        }
        [TestMethod]
        public void BuildWeightsTableTest1() {
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[0]);
            WeightsTable weightsTable = new HuffmanEncoder().BuildWeightsTable(inputStream);
            Assert.IsNotNull(weightsTable);
            AssertHelper.CollectionIsEmpty(weightsTable);
        }
        [TestMethod]
        public void BuildWeightsTableTest2() {
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[] {1});
            WeightsTable weightsTable = new HuffmanEncoder().BuildWeightsTable(inputStream);
            Assert.IsNotNull(weightsTable);
            WeightedSymbol[] expected = {new WeightedSymbol(1, 1)};
            CollectionAssert.AreEqual(expected, weightsTable.ToArray());
        }
        [TestMethod]
        public void BuildWeightsTableTest3() {
            byte[] data = @"aaaabbbccedddddeeeaabdcefffeffadc".ToByteArray();

            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            WeightsTable weightsTable = new HuffmanEncoder().BuildWeightsTable(inputStream);
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
        [TestMethod]
        public void BuildHuffmanTreeTest1() {
            WeightsTable weightsTable = new WeightsTable();
            HuffmanTreeBase huffmanTree = new HuffmanEncoder().BuildHuffmanTree(weightsTable);
            Assert.IsNotNull(huffmanTree);
            AssertHelper.CollectionIsEmpty(huffmanTree.FlattenValues());
        }
        [TestMethod]
        public void BuildHuffmanTreeTest2() {
            WeightsTable weightsTable = new WeightsTable();
            weightsTable.TrackSymbol(1);

            HuffmanTreeBase huffmanTree = new HuffmanEncoder().BuildHuffmanTree(weightsTable);
            Assert.IsNotNull(huffmanTree);
            WeightedSymbol?[] expected = {
                new WeightedSymbol(1, 1), null, null
            };
            AssertHelper.AreEqual(expected, huffmanTree.FlattenValues());
        }
        [TestMethod]
        public void BuildHuffmanTreeTest3() {
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
            
            HuffmanTreeBase huffmanTree = new HuffmanEncoder().BuildHuffmanTree(weightsTable);
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
            AssertHelper.AreEqual(expected, huffmanTree.FlattenValues());
        }
        [TestMethod]
        public void BuildHuffmanTreeTest4() {
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
            
            HuffmanTreeBase huffmanTree = new HuffmanEncoder().BuildHuffmanTree(weightsTable);
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
            AssertHelper.AreEqual(expected, huffmanTree.FlattenValues());
        }
        [TestMethod]
        public void BuildCodingTableTest1() {
            CodingTable codingTable = new HuffmanEncoder().BuildCodingTable(EmptyHuffmanTree.Instance);
            AssertHelper.CollectionIsEmpty(codingTable);
        }
        [TestMethod]
        public void BuildCodingTableTest2() {
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewNode(1, 'X').CreateTree();
            CodingTable codingTable = new HuffmanEncoder().BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);

            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'X', BitSequence.FromString("0"))
            };
            AssertHelper.AreEquivalent(expected, codingTable);
        }
        [TestMethod]
        public void BuildCodingTableTest3() {
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(15)
                .WithLeft(5, 'A')
                .WithRight(10, 'B')
                .CreateTree();

            CodingTable codingTable = new HuffmanEncoder().BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);
            
            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'A', BitSequence.FromString("0")),
                new Pair<byte, BitSequence>((byte)'B', BitSequence.FromString("1")),
            };
            AssertHelper.AreEquivalent(expected, codingTable);
        }
        [TestMethod]
        public void BuildCodingTableTest4() {
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

            CodingTable codingTable = new HuffmanEncoder().BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);
            
            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'a', BitSequence.FromString("001")),
                new Pair<byte, BitSequence>((byte)'b', BitSequence.FromString("0000")),
                new Pair<byte, BitSequence>((byte)'c', BitSequence.FromString("0001")),
                new Pair<byte, BitSequence>((byte)'d', BitSequence.FromString("010")),
                new Pair<byte, BitSequence>((byte)'e', BitSequence.FromString("011")),
                new Pair<byte, BitSequence>((byte)'f', BitSequence.FromString("1")),
            };
            AssertHelper.AreEquivalent(expected, codingTable);
        }
        [TestMethod]
        public void BuildCodingTableTest5() {
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

            CodingTable codingTable = new HuffmanEncoder().BuildCodingTable(tree);
            Assert.IsNotNull(codingTable);

            Pair<byte, BitSequence>[] expected = {
                new Pair<byte, BitSequence>((byte)'A', BitSequence.FromString("1101")),
                new Pair<byte, BitSequence>((byte)'B', BitSequence.FromString("10")),
                new Pair<byte, BitSequence>((byte)'C', BitSequence.FromString("0")),
                new Pair<byte, BitSequence>((byte)'D', BitSequence.FromString("1100")),
                new Pair<byte, BitSequence>((byte)'E', BitSequence.FromString("111")),
            };
            AssertHelper.AreEquivalent(expected, codingTable);
        }
        [TestMethod]
        public void EncodeTest1() {
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[0]);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();

            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            encoder.Encode(inputStream, outputStream, token);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);
            AssertHelper.CollectionIsEmpty(token.HuffmanTree.FlattenValues());
            AssertHelper.CollectionIsEmpty(outputStream.BitList);
        }
        [TestMethod]
        public void EncodeTest2() {
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[] { 1 });
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            encoder.Encode(inputStream, outputStream, token);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);

            WeightedSymbol?[] expected = {
                new WeightedSymbol(1, 1), 
                null,
                null,
            };
            AssertHelper.AreEqual(expected, token.HuffmanTree.FlattenValues());
            AssertOutputStream(outputStream, "0");
        }
        [TestMethod]
        public void EncodeTest3() {
            byte[] data = { 1, 2, 2, 1, 1, 2, 2 };
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            encoder.Encode(inputStream, outputStream, token);
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
            AssertHelper.AreEqual(expected, token.HuffmanTree.FlattenValues());
            AssertOutputStream(outputStream, "0110011");
        }
        [TestMethod]
        public void EncodeTest4() {
            byte[] data = { 1, 2, 2, 2, 2, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4 };
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();
            
            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            encoder.Encode(inputStream, outputStream, token);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);
            AssertOutputStream(outputStream, "110111111111111110111111111101010101010001010000000000");
        }

        private void AssertOutputStream(TestIEncodingOutputStream stream, string expected) {
            IReadOnlyList<Bit> bitList = stream.BitList;
            if(bitList.Count != expected.Length) throw new AssertFailedException();

            for(int n = 0; n < bitList.Count; n++) {
                if(bitList[n] != CharToBit(expected[n]))
                    throw new AssertFailedException();
            }
        }
        private static Bit CharToBit(char @char) {
            switch(@char) {
                case '0': return Bit.Zero;
                case '1': return Bit.One;
                default: throw new ArgumentException();    
            }
        }

        private class EmptyEncodingToken : EncodingToken {
            private EmptyEncodingToken()
                : base(EmptyHuffmanTree.Instance, new CodingTable()) {
            }
            public static readonly EmptyEncodingToken Instance = new EmptyEncodingToken();
        }
    }
}
#endif