using System;
using System.Collections.Generic;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;
using NUnit.Framework;

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
        long IDecodingInputStream.SizeInBytes {
            get { return data.Count; }
        }
        long IDecodingInputStream.Position {
            get { return index; }
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
        string IDecodingOutputStream.Path {
            get { return "[Test]"; }
        }
        void IDisposable.Dispose() {
        }
        
        #endregion

        public IReadOnlyList<byte> ByteList { get { return byteList; } }
    }

    #endregion

    [TestFixture]
    public class HuffmanDecoderTests {
        [Test]
        public void DecodeGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(null, HuffmanTreeHelper.SomeTree(), new TestIDecodingOutputStream(), 1, null);
            });
        }
        [Test]
        public void DecodeGuardCase2Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), (HuffmanTree)null, new TestIDecodingOutputStream(), 1, null);
            });
        }
        [Test]
        public void DecodeGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), HuffmanTreeHelper.SomeTree(), null, 1, null);
            });
        }
        [Test]
        public void DecodeGuardCase4Test() {
            Assert.Throws<ArgumentException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), HuffmanTreeHelper.SomeTree(), new TestIDecodingOutputStream(), -1, null);
            });
        }
        [Test]
        public void DecodeGuardCase5Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(null, new WeightsTable(), new TestIDecodingOutputStream(), 1L, null);
            });
        }
        [Test]
        public void DecodeGuardCase6Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), (WeightsTable)null, new TestIDecodingOutputStream(), 1L, null);
            });
        }
        [Test]
        public void DecodeGuardCase7Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), new WeightsTable(), null, 1L, null);
            });
        }
        [Test]
        public void DecodeGuardCase8Test() {
            Assert.Throws<ArgumentException>(() => {
                new HuffmanDecoder().Decode(new TestIDecodingInputStream(new Bit[0]), new WeightsTable(), new TestIDecodingOutputStream(), -1, null);
            });
        }
        [Test]
        public void DecodeTest1() {
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(new Bit[0]);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();

            new HuffmanDecoder().Decode(inputStream, HuffmanTreeHelper.SomeTree(), outputStream, 0, null);
            CollectionAssert.IsEmpty(outputStream.ByteList);
        }
        [Test]
        public void DecodeTest2() {
            Bit[] data = TestHelper.BitsFromString("00000");
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(data);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();
            HuffmanTree tree = HuffmanTreeHelper.Builder().NewNode(1, 1).CreateTree();

            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 5, null);
            AssertOutputStream(outputStream, 1, 1, 1, 1, 1);
        }
        [Test]
        public void DecodeTest3() {
            Bit[] data = TestHelper.BitsFromString("0110011");
            TestIDecodingInputStream inputStream = new TestIDecodingInputStream(data);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();

            HuffmanTree tree = HuffmanTreeHelper.Builder().NewInternalNode(7)
                .WithLeft(3, 1)
                .WithRight(4, 2)
                .CreateTree();

            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 7, null);
            AssertOutputStream(outputStream, 1, 2, 2, 1, 1, 2, 2);
        }
        [Test]
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
            
            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 30, null);
            AssertOutputStream(outputStream, 3, 1, 2, 2, 2, 2, 3, 3, 3, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3);
        }
        [Test]
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

            Assert.Throws<ArgumentException>(() => new HuffmanDecoder().Decode(inputStream, tree, outputStream, 30, null));
        }
        [Test]
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

            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 26, null);
            AssertOutputStream(outputStream, 4, 4, 1, 2, 2, 1, 1, 2, 3, 2, 2, 2, 2, 1, 1);
        }
        [Test]
        public void DecodingProgressTest1() {
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

            TestIProgressHandler progressHandler = new TestIProgressHandler();
            new HuffmanDecoder().Decode(inputStream, tree, outputStream, 26, progressHandler);
            CollectionAssert.IsEmpty(progressHandler.ValueList);
        }
        [Test]
        public void DecodingProgressTest2() {
            string inputString = TestHelper.CreateString("10110111001111010000001111", 100000);
            Bit[] data = TestHelper.BitsFromString(inputString);
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

            TestIProgressHandler progress = new TestIProgressHandler();
            HuffmanDecoder decoder = new HuffmanDecoder();

            for(int n = 0; n < 10; n++) {
                decoder.Decode(inputStream, tree, outputStream, 260000, progress);
            }
            CollectionAssert.AreEqual(new long[] { 131072, 131072 }, progress.ValueList);
        }

        private void AssertOutputStream(TestIDecodingOutputStream stream, params byte[] expected) {
            IReadOnlyList<byte> bitList = stream.ByteList;
            if(bitList.Count != expected.Length) throw new AssertionException(nameof(stream));

            for(int n = 0; n < bitList.Count; n++) {
                if(bitList[n] != expected[n])
                    throw new AssertionException(nameof(stream));
            }
        }
    }
}
