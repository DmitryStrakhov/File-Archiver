using System;
using System.Collections.Generic;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {

    #region Streams

    class TestIDecodingInputStream : IDecodingInputStream {
        readonly IReadOnlyList<Bit> data;
        int index;

        public TestIDecodingInputStream(IReadOnlyList<Bit> data) {
            Guard.IsNotNull(data, nameof(data));
            this.index = 0;
            this.data = data;
        }

        #region IDecodingInputStream

        bool IDecodingInputStream.ReadBit(out Bit bit) {
            bool hasBit = index < data.Count;
            bit = hasBit ? data[index++] : Bit.Zero;
            return hasBit;
        }
        bool IDecodingInputStream.IsEmpty {
            get {
                if(index < data.Count) return false;
                return true;
            }
        }
        void IDisposable.Dispose() {
        }

        #endregion
    }

    class TestIDecodingOutputStream : IDecodingOutputStream {
        readonly List<byte> byteList;

        public TestIDecodingOutputStream() {
            this.byteList = new List<byte>(64);
        }

        #region IDecodingOutputStream

        void IDecodingOutputStream.WriteSymbol(byte symbol) {
            byteList.Add(symbol);
        }
        void IDisposable.Dispose() {
        }
        
        #endregion

        public IReadOnlyList<byte> ByteList { get { return byteList; } }
    }

    #endregion

    [TestClass]
    public class HuffmanDecoderTests {
        [TestMethod]
        public void DecodeGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(null, HuffmanTreeHelper.SomeTree(), new TestIDecodingOutputStream(), 1);
            });
        }
        [TestMethod]
        public void DecodeGuardCase2Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), (HuffmanTree)null, new TestIDecodingOutputStream(), 1);
            });
        }
        [TestMethod]
        public void DecodeGuardCase3Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), HuffmanTreeHelper.SomeTree(), null, 1);
            });
        }
        [TestMethod]
        public void DecodeGuardCase4Test() {
            AssertHelper.Throws<ArgumentException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), HuffmanTreeHelper.SomeTree(), new TestIDecodingOutputStream(), -1);
            });
        }
        [TestMethod]
        public void DecodeGuardCase5Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(null, new WeightsTable(), new TestIDecodingOutputStream(), 1L);
            });
        }
        [TestMethod]
        public void DecodeGuardCase6Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), (WeightsTable)null, new TestIDecodingOutputStream(), 1L);
            });
        }
        [TestMethod]
        public void DecodeGuardCase7Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), new WeightsTable(), null, 1L);
            });
        }
        [TestMethod]
        public void DecodeGuardCase8Test() {
            AssertHelper.Throws<ArgumentException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), new WeightsTable(), new TestIDecodingOutputStream(), -1);
            });
        }
        [TestMethod]
        public void DecodeTest1() {
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(new Bit[0]);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();

            new HuffmanDecoder().Decode(inputStream, HuffmanTreeHelper.SomeTree(), outputStream, 0);
            AssertHelper.CollectionIsEmpty(outputStream.ByteList);
        }
        [TestMethod]
        public void DecodeTest2() {
            Bit[] data = TestHelper.BitsFromString("00000");
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(data);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewNode(1, 1).CreateTree();

            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 5);
            AssertOutputStream(outputStream, 1, 1, 1, 1, 1);
        }
        [TestMethod]
        public void DecodeTest3() {
            Bit[] data = TestHelper.BitsFromString("0110011");
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(data);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();

            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(7)
                .WithLeft(3, 1)
                .WithRight(4, 2)
                .CreateTree();

            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 7);
            AssertOutputStream(outputStream, 1, 2, 2, 1, 1, 2, 2);
        }
        [TestMethod]
        public void DecodeTest4() {
            Bit[] data = TestHelper.BitsFromString("110101010101111110000000111111");
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(data);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();
            
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(19)
                .WithLeft(8, 1)
                .WithRightInternal(11)
                .GoToRight()
                .WithLeft(4, 2)
                .WithRight(7, 3)
                .CreateTree();
            
            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 30);
            AssertOutputStream(outputStream, 3, 1, 2, 2, 2, 2, 3, 3, 3, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3);
        }
        [TestMethod]
        public void DecodeTest5() {
            Bit[] data = TestHelper.BitsFromString("110101010101111110000000111111");
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(data);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();
            
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(1)
                .WithRightInternal(1)
                .WithLeftInternal(1)
                .GoToLeft()
                .WithLeftInternal(1)
                .GoToLeft()
                .WithLeftInternal(1)
                .GoToLeft()
                .WithLeft(1, 5)
                .WithRight(1, 6)
                .CreateTree();

            AssertHelper.Throws<ArgumentException>(() => new HuffmanDecoder().Decode(inputStream, tree, outputStream, 30));
        }
        [TestMethod]
        public void DecodeTest6() {
            Bit[] data = TestHelper.BitsFromString("10110111001111010000001111");
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(data);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();

            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(15)
                .WithLeft(7, 2)
                .WithRightInternal(8)
                .GoToRight()
                .WithRight(5, 1)
                .WithLeftInternal(3)
                .GoToLeft()
                .WithLeft(1, 3)
                .WithRight(2, 4)
                .CreateTree();

            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 26);
            AssertOutputStream(outputStream, 4, 4, 1, 2, 2, 1, 1, 2, 3, 2, 2, 2, 2, 1, 1);
        }

        private void AssertOutputStream(TestIDecodingOutputStream stream, params byte[] expected) {
            IReadOnlyList<byte> bitList = stream.ByteList;
            if(bitList.Count != expected.Length) throw new AssertFailedException();

            for(int n = 0; n < bitList.Count; n++) {
                if(bitList[n] != expected[n])
                    throw new AssertFailedException();
            }
        }
    }
}
