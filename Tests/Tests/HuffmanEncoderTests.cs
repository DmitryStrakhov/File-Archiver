using System;
using System.Collections.Generic;
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
                new HuffmanEncoder().Encode(null, new TestIEncodingOutputStream(), EmptyEncodingToken.Instance)
            );
        }
        [Test]
        public void EncodeGuardCase2Test() {
            Assert.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(new TestIEncodingInputStream(new byte[0]), null, EmptyEncodingToken.Instance)
            );
        }
        [Test]
        public void EncodeGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() =>
                new HuffmanEncoder().Encode(new TestIEncodingInputStream(new byte[0]), new TestIEncodingOutputStream(), null)
            );
        }
        
        [Test]
        public void EncodeTest1() {
            TestIEncodingInputStream inputStream = new TestIEncodingInputStream(new byte[0]);
            TestIEncodingOutputStream outputStream = new TestIEncodingOutputStream();

            HuffmanEncoder encoder = new HuffmanEncoder();
            EncodingToken token = encoder.CreateEncodingToken(inputStream);
            long length = encoder.Encode(inputStream, outputStream, token);
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
            long length = encoder.Encode(inputStream, outputStream, token);
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
            long length = encoder.Encode(inputStream, outputStream, token);
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
            long length = encoder.Encode(inputStream, outputStream, token);
            Assert.IsNotNull(token.HuffmanTree);
            Assert.IsNotNull(token.CodingTable);
            Assert.AreEqual(54, length);
            AssertOutputStream(outputStream, "110111111111111110111111111101010101010001010000000000");
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
